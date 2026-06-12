using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.AST;
using Spelllang.Lexer;

namespace Spelllang.Tests.Parser
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Parse_FunctionCall()
        {
            var input = new List<Token>
            {
                new(Type.IDENTIFIER, "PRINT"),
                new(Type.PARENTHESES_LEFT, "("),
                new(Type.STRING, "1"),
                new(Type.COMMA, ","),
                new(Type.STRING, " + "),
                new(Type.COMMA, ","),
                new(Type.NUMBER, "2"),
                new(Type.PARENTHESES_RIGHT, ")")
            };
            var expected = new ProgramNode(new List<IStatementNode>
            {
                new ExpressionStatement(
                    new CallExpression(
                        new IdentifierExpression("PRINT"),
                        new List<IExpressionNode>
                        {
                            new StringExpression("1"),
                            new StringExpression(" + "),
                            new IntegerExpression(2)
                        }
                    )
                )
            });
            ParsingTestUtils.AssertAst(ParsingTestUtils.Parse(input), expected);
        }
    }
}