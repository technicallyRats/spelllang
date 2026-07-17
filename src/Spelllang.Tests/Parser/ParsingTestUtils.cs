using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.AST;
using Spelllang.Lexer;

namespace Spelllang.Tests.Parser
{
    public static class ParsingTestUtils
    {
        public static void AssertParserState(Spelllang.Parser.Parser actual, ProgramNode expected)
        {
            if (actual.IsFaulty())
            {
                actual.DisplayErrors(null);
                Assert.IsTrue(false);
            }

            Assert.That(actual.GetRootProgram().ToReadableString(), Is.EqualTo(expected.ToReadableString()).NoClip);
        }

        public static Spelllang.Parser.Parser Parse(List<Token> input)
        {
            var enumerator = new TokenEnumerator(input);
            var parser = new Spelllang.Parser.Parser(enumerator);
            return parser;
        }

        public static ProgramNode BuildCallProgramNode(string identifierName, List<IExpressionNode> args)
        {
            return new ProgramNode(new List<IStatementNode>
            {
                new ExpressionStatement(BuildCallExpression(identifierName, args))
            });
        }

        public static CallExpression BuildCallExpression(string identifierName, List<IExpressionNode> args)
        {
            return new CallExpression(
                new IdentifierExpression(identifierName),
                args
            );
        }
    }
}