using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.AST;
using Spelllang.Lexer;
using Spelllang.Tests.TestUtils;
using Type = Spelllang.Lexer.Type;

namespace Spelllang.Tests.Parser
{
    public class FunctionParsingTests
    {
        public static IEnumerable FunctionTestCases
        {
            get
            {
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.FUNCTION, "function", inc.Increment("function")),
                            new(Type.IDENTIFIER, "Test", inc.Increment("Test")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}"))
                        },
                        new ProgramNode(new List<IStatementNode>
                        {
                            new FunctionStatement(
                                "Test",
                                new List<IdentifierExpression>
                                {
                                },
                                new ProgramNode(
                                    new List<IStatementNode>
                                    {
                                    }
                                )
                            )
                        })
                    ).SetName("Simple function with no body");
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.FUNCTION, "function", inc.Increment("function")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}"))
                        },
                        new ProgramNode(new List<IStatementNode>
                        {
                            new FunctionStatement(
                                "anon",
                                new List<IdentifierExpression>
                                {
                                },
                                new ProgramNode(
                                    new List<IStatementNode>
                                    {
                                    }
                                )
                            )
                        })
                    ).SetName("Simple anon function with no body");
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.FUNCTION, "function", inc.Increment("function")),
                            new(Type.IDENTIFIER, "Test", inc.Increment("Test")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.IDENTIFIER, "a", inc.Increment("a")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.RETURN, "return", inc.Increment("return")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}"))
                        },
                        new ProgramNode(new List<IStatementNode>
                        {
                            new FunctionStatement(
                                "Test",
                                new List<IdentifierExpression>
                                {
                                    new("a")
                                },
                                new ProgramNode(
                                    new List<IStatementNode>
                                    {
                                        new ReturnStatement(new NullExpression())
                                    }
                                )
                            )
                        })
                    ).SetName("Function with arguments and return without value");
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.FUNCTION, "function", inc.Increment("function")),
                            new(Type.IDENTIFIER, "Test", inc.Increment("Test")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.IDENTIFIER, "a", inc.Increment("a")),
                            new(Type.COMMA, ",", inc.Increment(",")),
                            new(Type.IDENTIFIER, "b", inc.Increment("b")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.RETURN, "return", inc.Increment("return")),
                            new(Type.IDENTIFIER, "a", inc.Increment("a")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}"))
                        },
                        new ProgramNode(new List<IStatementNode>
                        {
                            new FunctionStatement(
                                "Test",
                                new List<IdentifierExpression>
                                {
                                    new("a"),
                                    new("b")
                                },
                                new ProgramNode(
                                    new List<IStatementNode>
                                    {
                                        new ReturnStatement(
                                            new IdentifierExpression("a")
                                        )
                                    }
                                )
                            )
                        })
                    ).SetName("Function with arguments and return value");
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.FUNCTION, "function", inc.Increment("function")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.IDENTIFIER, "a", inc.Increment("a")),
                            new(Type.COMMA, ",", inc.Increment(",")),
                            new(Type.IDENTIFIER, "b", inc.Increment("b")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.RETURN, "return", inc.Increment("return")),
                            new(Type.IDENTIFIER, "a", inc.Increment("a")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}"))
                        },
                        new ProgramNode(new List<IStatementNode>
                        {
                            new FunctionStatement(
                                "anon",
                                new List<IdentifierExpression>
                                {
                                    new("a"),
                                    new("b")
                                },
                                new ProgramNode(
                                    new List<IStatementNode>
                                    {
                                        new ReturnStatement(
                                            new IdentifierExpression("a")
                                        )
                                    }
                                )
                            )
                        })
                    ).SetName("Anon function with arguments and return value");
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(FunctionTestCases))]
        public void Parse_Function(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertAst(ParsingTestUtils.Parse(input), expected);
        }
    }
}