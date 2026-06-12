using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Spelllang.AST;
using Spelllang.Lexer;

namespace Spelllang.Tests.Parser
{
    [TestFixture]
    public class ParserTests
    {
        public static IEnumerable CallTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.IDENTIFIER, "PRINT"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.PARENTHESES_RIGHT, ")")
                    },
                    ParsingTestUtils.BuildCallProgramNode("PRINT",
                        new List<IExpressionNode>())
                ).SetName("Simple function call");
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.IDENTIFIER, "PRINT"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.STRING, "1"),
                        new(Type.COMMA, ","),
                        new(Type.STRING, " + "),
                        new(Type.COMMA, ","),
                        new(Type.NUMBER, "2"),
                        new(Type.PARENTHESES_RIGHT, ")")
                    },
                    ParsingTestUtils.BuildCallProgramNode("PRINT", new List<IExpressionNode>
                    {
                        new StringExpression("1"),
                        new StringExpression(" + "),
                        new IntegerExpression(2)
                    })
                ).SetName("Simple args function call");
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.IDENTIFIER, "PRINT"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.IDENTIFIER, "GETVAL"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.NUMBER, "2"),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.COMMA, ","),
                        new(Type.IDENTIFIER, "ABC"),
                        new(Type.NUMBER, "1"),
                        new(Type.PARENTHESES_RIGHT, ")")
                    },
                    ParsingTestUtils.BuildCallProgramNode("PRINT", new List<IExpressionNode>
                    {
                        ParsingTestUtils.BuildCallExpression("GETVAL", new List<IExpressionNode>
                        {
                            new IntegerExpression(2)
                        }),
                        new IdentifierExpression("ABC"),
                        new IntegerExpression(1)
                    })
                ).SetName("Nested args function call");
            }
        }

        [Test]
        [TestCaseSource(nameof(CallTestCases))]
        public void Parse_FunctionCall(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertAst(ParsingTestUtils.Parse(input), expected);
        }
    }
}