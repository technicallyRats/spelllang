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

        public IRuntimeVariableBase Run()
        {
            return RunProgram(RootNode, new Context(null, Builtins)).result;
        }

        private (IRuntimeVariableBase result, bool earlyExit) RunProgram(ProgramNode program, Context _Context)
        {
            IRuntimeVariableBase result = new RuntimeNull();
            foreach (var statement in program.Statements)
            {
                // would just clutter the return stack
                if (statement is ExpressionStatement expressionStatement)
                    if (expressionStatement.Expression is SemicolonExpression)
                        continue;

                result = RunNode(statement, _Context);
                if (statement is ReturnStatement or BreakStatement) return (result, true);
            }

            return (result, false);
        }

        private IRuntimeVariableBase RunNode(IAstNode node, Context _Context)
        {
            if (node is ProgramNode programNode) return RunProgram(programNode, _Context).result;

            if (node is IStatementNode statementNode) return RunStatement(statementNode, _Context);
            SpelllangDiagnostics.Error("Error occured: Could not run node of type " + node);
            return new RuntimeNull();
        }

        private IRuntimeVariableBase RunStatement(IAstNode statement, Context _Context)
        {
            switch (statement)
            {
                case ExpressionStatement expressionStatement:
                    return RunExpression(expressionStatement.Expression, _Context);
                case FunctionStatement functionStatement:
                    var function =
                        new RuntimeFunction(functionStatement.Program,
                            functionStatement.ArgumentIdentifiers.Select(identifier => identifier.IdentifierName)
                                .ToList());
                    if (!functionStatement.isAnonymous()) _Context.Register(functionStatement.FunctionName, function);
                    return function;
                case IfStatement ifStatement:
                    var test = RunExpression(ifStatement.Condition, _Context);
                    if (Operations.IsTruthy(RunExpression(ifStatement.Condition, _Context)))
                        return RunProgram(ifStatement.Primary, _Context).result;
                    if (ifStatement.HasSecondary())
                        return RunProgram(ifStatement.Secondary, _Context).result;
                    return new RuntimeNull();
                case WhileStatement whileStatement:
                    (IRuntimeVariableBase result, bool earlyExit) result = (new RuntimeNull(), false);
                    while (Operations.IsTruthy(RunExpression(whileStatement.Condition, _Context)) && !result.earlyExit)
                        result = RunProgram(whileStatement.Body, _Context);
                    return result.result;
                case ReturnStatement returnStatement:
                    return RunExpression(returnStatement.ReturnValue, _Context);
                case BreakStatement:
                    return new RuntimeNull();
                case IExpressionNode expressionNode:
                    return RunExpression(expressionNode, _Context);
                case ImportStatement importStatement:
                    // Construct new, pure context
                    var importContext = new Context(null, Builtins);
                    var importRoot = ImportUtils.ReadAndParseInput(importStatement.ImportPath, Resolvers);
                    // populate context -> this cannot(!) modify the current context just yet
                    var _ = RunProgram(importRoot, importContext);
                    // We do not need builtin duplicates
                    foreach (var builtin in Builtins) importContext.Evict(builtin.name);
                    _Context.Merge(importContext, importStatement.ImportPrefix, true);
                    return new RuntimeNull();
                default:
                    SpelllangDiagnostics.Error("Unknown statement type " + statement);
                    return null;
            }
        }

        private IRuntimeVariableBase RunExpression(IAstNode expression, Context _Context)
        {
            switch (expression)
            {
                case AssignExpression assignNode:
                    var value = RunExpression(assignNode.Value, _Context);
                    _Context.Register(assignNode.Identifier, value);
                    return value;
                case CallExpression callNode:
                    var node = RunExpression(callNode.Identifier, _Context);
                    if (node == null)
                    {
                        SpelllangDiagnostics.Error("Undefined function: " + callNode.Identifier.ToReadableString());
                        return new RuntimeNull();
                    }

                    switch (node)
                    {
                        case RuntimeFunction fnNode: return RunFunction(fnNode, callNode, _Context);
                        case IRuntimeBuiltin builtinNode: return RunBuiltin(builtinNode, callNode, _Context);
                        default:
                            SpelllangDiagnostics.Error("Unexpected resolution for function call: " +
                                                       node.ToReadableString());
                            return new RuntimeNull();
                    }
                case InfixExpression infixNode:
                    return Operations.RunInfixExpression(infixNode.Operator, RunStatement(infixNode.Left, _Context),
                        RunStatement(infixNode.Right, _Context));
                case PrefixExpression prefixNode:
                    return Operations.RunPrefixExpression(prefixNode.Operator,
                        RunStatement(prefixNode.Right, _Context));
                case BooleanExpression booleanNode: return new RuntimeBoolean(booleanNode.Value);
                case StringExpression stringNode: return new RuntimeString(stringNode.Value);
                case FloatExpression floatNode: return new RuntimeFloat(floatNode.Value);
                case IntegerExpression integerNode: return new RuntimeInt(integerNode.Value);
                case NullExpression _: return new RuntimeNull();
                case IdentifierExpression identifierExpression:
                    return _Context.Retrieve(identifierExpression.IdentifierName);
                default:
                    SpelllangDiagnostics.Error("Unknown expression type " + expression);
                    return null;
            }
        }

        private IRuntimeVariableBase RunFunction(RuntimeFunction program, CallExpression callNode, Context _Context)
        {
            var callContext = _Context.Enclose();
            var arguments = new List<IRuntimeVariableBase>();
            for (var i = 0; i < Math.Max(callNode.Arguments.Count, program.GetArgumentNames().Count); i++)
                // This will fail if there is an argument mismatch. Good.
                callContext.Register(program.GetArgumentNames()[i], RunExpression(callNode.Arguments[i], _Context));
            return RunProgram(program.GetValue(), callContext).result;
        }

        private IRuntimeVariableBase RunBuiltin(IRuntimeBuiltin builtin, CallExpression callNode, Context _Context)
        {
            var arguments = new List<IRuntimeVariableBase>();
            for (var i = 0; i < callNode.Arguments.Count; i++)
                arguments.Add(RunExpression(callNode.Arguments[i], _Context));
            return builtin.Call(arguments);
        }
    }
}