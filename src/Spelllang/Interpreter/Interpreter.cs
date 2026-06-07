using System;
using System.Collections.Generic;
using Spelllang.AST;
using Spelllang.Builtins;

namespace Spelllang.Interpreter
{
    public class Interpreter
    {
        private ProgramNode RootNode;

        private Parser.Parser _Parser;

        private List<(string name, Builtins.IRuntimeBuiltin value)> Builtins;

        public Interpreter(Parser.Parser parser)
        {
            _Parser = parser;
            Builtins = new List<(string name, IRuntimeBuiltin value)>();
            RootNode = _Parser.GetRootProgram();
        }

        public Interpreter(Parser.Parser parser, List<(string name, Builtins.IRuntimeBuiltin value)> builtins)
        {
            _Parser = parser;
            Builtins = builtins;
            RootNode = _Parser.GetRootProgram();
        }

        public IRuntimeVariableBase Run()
        {
            return RunProgram(RootNode, new Context(null, Builtins));
        }

        private IRuntimeVariableBase RunProgram(ProgramNode program, Context _Context)
        {
            IRuntimeVariableBase result = null;
            foreach (IStatementNode statement in program.Statements)
            {
                // would just clutter the return stack
                if (statement is ExpressionStatement expressionStatement)
                {
                    if (expressionStatement.Expression is SemicolonExpression) { continue; }
                }
                result = RunNode(statement, _Context);
            }
            return result;
        }

        private IRuntimeVariableBase RunNode(IAstNode node, Context _Context)
        {
            if (node is ProgramNode programNode)
            {
                return RunProgram(programNode, _Context);
            }

            if (node is IStatementNode statementNode)
            {
                return RunStatement(statementNode, _Context);
            }
            Console.WriteLine("Error occured: Could not run node of type " + node);
            return new RuntimeNull();
        }

        private IRuntimeVariableBase RunStatement(IAstNode statement, Context _Context)
        {
            switch (statement)
            {
                case ExpressionStatement expressionStatement: return RunExpression(expressionStatement.Expression, _Context);
                case IExpressionNode expressionNode: return RunExpression(expressionNode, _Context);
                default: Console.WriteLine("Unknown statement type " + statement); return null;
            }
        }

        private IRuntimeVariableBase RunExpression(IAstNode expression, Context _Context)
        {
            switch (expression)
            {
                case AssignExpression assignNode:
                    IRuntimeVariableBase value = RunExpression(assignNode.Value, _Context);
                    _Context.Register(assignNode.Identifier, value);
                    return value;
                case CallExpression callNode:
                    IRuntimeVariableBase node = RunExpression(callNode.Identifier, _Context);
                    switch (node)
                    {
                        case RuntimeFunction fnNode: return RunFunction(fnNode, callNode, _Context);
                        case Builtins.IRuntimeBuiltin builtinNode: return RunBuiltin(builtinNode, callNode, _Context);
                        default:
                            Console.WriteLine("Unexpected resolution for function call " + node.GetType());
                            return new RuntimeNull();
                    }
                case InfixExpression infixNode:
                    return Operations.RunInfixExpression(infixNode.Operator, RunStatement(infixNode.Left, _Context), RunStatement(infixNode.Right, _Context));
                case BooleanExpression booleanNode: return new RuntimeBoolean(booleanNode.Value);
                case StringExpression stringNode: return new RuntimeString(stringNode.Value);
                case FloatExpression floatNode: return new RuntimeFloat(floatNode.Value);
                case IntegerExpression integerNode: return new RuntimeInt(integerNode.Value);
                case IdentifierExpression identifierExpression: return _Context.Retrieve(identifierExpression.IdentifierName);
                default: Console.WriteLine("Unknown expression type " + expression); return null;
            }
        }

        private IRuntimeVariableBase RunFunction(RuntimeFunction program, CallExpression callNode, Context _Context)
        {
            Context callContext = _Context.Enclose();
            List<IRuntimeVariableBase> arguments = new List<IRuntimeVariableBase>();
            for (int i = 0; i < Math.Max(callNode.Arguments.Count, program.GetArgumentNames().Count); i++)
            {
                // This will fail if there is an argument mismatch. Good.
                callContext.Register(program.GetArgumentNames()[i], RunExpression(callNode.Arguments[i], _Context));
            }
            return RunProgram(program.GetValue(), callContext);
        }

        private IRuntimeVariableBase RunBuiltin(Builtins.IRuntimeBuiltin builtin, CallExpression callNode, Context _Context)
        {
            List<IRuntimeVariableBase> arguments = new List<IRuntimeVariableBase>();
            for (int i = 0; i < callNode.Arguments.Count; i++)
            {
                arguments.Add(RunExpression(callNode.Arguments[i], _Context));
            }
            return builtin.Call(arguments);
        }
    }
}
