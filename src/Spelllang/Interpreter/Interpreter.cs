using System;
using System.Collections.Generic;
using System.Linq;
using Spelllang.AST;
using Spelllang.Builtins;
using Spelllang.Diagnostics;
using Spelllang.Interpreter.Importer;

namespace Spelllang.Interpreter
{
    public class Interpreter
    {
        private readonly Parser.Parser _Parser;

        private readonly List<(string name, IRuntimeBuiltin value)> Builtins;
        private readonly List<IImportResolver> Resolvers;
        private readonly ProgramNode RootNode;

        public Interpreter(Parser.Parser parser)
        {
            _Parser = parser;
            Builtins = new List<(string name, IRuntimeBuiltin value)>();
            Resolvers = new List<IImportResolver>();
            RootNode = _Parser.GetRootProgram();
        }

        public Interpreter(Parser.Parser parser, List<(string name, IRuntimeBuiltin value)> builtins)
        {
            _Parser = parser;
            Builtins = builtins;
            Resolvers = new List<IImportResolver>();
            RootNode = _Parser.GetRootProgram();
        }

        public Interpreter(Parser.Parser parser, List<(string name, IRuntimeBuiltin value)> builtins,
            List<IImportResolver> resolvers)
        {
            _Parser = parser;
            Builtins = builtins;
            Resolvers = resolvers;
            RootNode = _Parser.GetRootProgram();
        }

        public IRuntimeVariableBase? Run()
        {
            if (!_Parser.IsFaulty()) return RunProgram(RootNode, new Context(null, Builtins)).result;
            SpelllangDiagnostics.Error("I refuse to do this! I shall be fed appropriately");
            return null;
        }

        private (IRuntimeVariableBase result, bool earlyExit) RunProgram(ProgramNode program, Context context)
        {
            IRuntimeVariableBase result = new RuntimeNull();
            foreach (var statement in program.Statements)
            {
                // would just clutter the return stack
                if (statement is ExpressionStatement expressionStatement)
                    if (expressionStatement.Expression is SemicolonExpression)
                        continue;

                result = RunNode(statement, context);
                if (statement is ReturnStatement or BreakStatement) return (result, true);
                if (result is RuntimeError) return (result, true);
            }

            return (result, false);
        }

        private IRuntimeVariableBase RunNode(IAstNode node, Context context)
        {
            if (node is ProgramNode programNode) return RunProgram(programNode, context).result;

            if (node is IStatementNode statementNode) return RunStatement(statementNode, context);
            return new RuntimeError("Error occured: Could not run node of type {node}");
        }

        private IRuntimeVariableBase RunStatement(IAstNode statement, Context context)
        {
            switch (statement)
            {
                case ExpressionStatement expressionStatement:
                    return RunExpression(expressionStatement.Expression, context);
                case FunctionStatement functionStatement:
                    var function =
                        new RuntimeFunction(functionStatement.Program,
                            functionStatement.ArgumentIdentifiers.Select(identifier => identifier.IdentifierName)
                                .ToList());
                    if (!functionStatement.isAnonymous()) context.Register(functionStatement.FunctionName, function);
                    return function;
                case IfStatement ifStatement:
                    var test = RunExpression(ifStatement.Condition, context);
                    if (Operations.IsTruthy(RunExpression(ifStatement.Condition, context)))
                        return RunProgram(ifStatement.Primary, context).result;
                    if (ifStatement.HasSecondary())
                        return RunProgram(ifStatement.Secondary, context).result;
                    return new RuntimeNull();
                case WhileStatement whileStatement:
                    (IRuntimeVariableBase result, bool earlyExit) result = (new RuntimeNull(), false);
                    while (Operations.IsTruthy(RunExpression(whileStatement.Condition, context)) && !result.earlyExit)
                        result = RunProgram(whileStatement.Body, context);
                    return result.result;
                case ReturnStatement returnStatement:
                    return RunExpression(returnStatement.ReturnValue, context);
                case BreakStatement:
                    return new RuntimeNull();
                case IExpressionNode expressionNode:
                    return RunExpression(expressionNode, context);
                case ImportStatement importStatement:
                    // Construct new, pure context
                    var importContext = new Context(null, Builtins);
                    var importRoot = ImportUtils.ReadAndParseInput(importStatement.ImportPath, Resolvers);
                    // populate context -> this cannot(!) modify the current context just yet
                    var _ = RunProgram(importRoot, importContext);
                    // We do not need builtin duplicates
                    foreach (var builtin in Builtins) importContext.Evict(builtin.name);
                    context.Merge(importContext, importStatement.ImportPrefix, true);
                    return new RuntimeNull();
                default:
                    return new RuntimeError($"Unknown statement type {statement}");
            }
        }

        private IRuntimeVariableBase RunExpression(IAstNode expression, Context context)
        {
            switch (expression)
            {
                case AssignExpression assignNode:
                    var value = RunExpression(assignNode.Value, context);
                    context.Register(assignNode.Identifier, value);
                    return value;
                case CallExpression callNode:
                    var node = RunExpression(callNode.Identifier, context);

                    switch (node)
                    {
                        case RuntimeFunction fnNode: return RunFunction(fnNode, callNode, context);
                        case IRuntimeBuiltin builtinNode: return RunBuiltin(builtinNode, callNode, context);
                        default:
                            return new RuntimeError(
                                $"Unexpected resolution for function call: {node.ToReadableString()}");
                    }
                case InfixExpression infixNode:
                    return Operations.RunInfixExpression(infixNode.Operator, RunStatement(infixNode.Left, context),
                        RunStatement(infixNode.Right, context));
                case PrefixExpression prefixNode:
                    return Operations.RunPrefixExpression(prefixNode.Operator,
                        RunStatement(prefixNode.Right, context));
                case BooleanExpression booleanNode: return new RuntimeBoolean(booleanNode.Value);
                case StringExpression stringNode: return new RuntimeString(stringNode.Value);
                case FloatExpression floatNode: return new RuntimeFloat(floatNode.Value);
                case IntegerExpression integerNode: return new RuntimeInt(integerNode.Value);
                case NullExpression _: return new RuntimeNull();
                case IdentifierExpression identifierExpression:
                    return context.Retrieve(identifierExpression.IdentifierName) ??
                           new RuntimeError($"Unknown variable {identifierExpression.IdentifierName}");
                default:
                    SpelllangDiagnostics.Error("Unknown expression type " + expression);
                    return new RuntimeError();
            }
        }

        private IRuntimeVariableBase RunFunction(RuntimeFunction program, CallExpression callNode, Context context)
        {
            var callContext = context.Enclose();
            var arguments = new List<IRuntimeVariableBase>();
            for (var i = 0; i < Math.Max(callNode.Arguments.Count, program.GetArgumentNames().Count); i++)
                // This will fail if there is an argument mismatch. Good.
                callContext.Register(program.GetArgumentNames()[i], RunExpression(callNode.Arguments[i], context));
            return RunProgram(program.GetValue(), callContext).result;
        }

        private IRuntimeVariableBase RunBuiltin(IRuntimeBuiltin builtin, CallExpression callNode, Context context)
        {
            var arguments = new List<IRuntimeVariableBase>();
            for (var i = 0; i < callNode.Arguments.Count; i++)
                arguments.Add(RunExpression(callNode.Arguments[i], context));
            return builtin.Call(arguments);
        }
    }
}