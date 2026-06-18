using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.AST;
using Spelllang.Lexer;
using Type = Spelllang.Lexer.Type;

namespace Spelllang.Tests.Parser
{
    public class FunctionParsingTests
    {
        public static IEnumerable FunctionTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.FUNCTION, "function"),
                        new(Type.IDENTIFIER, "Test"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.BRACES_RIGHT, "}")
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
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.FUNCTION, "function"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.BRACES_RIGHT, "}")
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
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.FUNCTION, "function"),
                        new(Type.IDENTIFIER, "Test"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.IDENTIFIER, "a"),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.RETURN, "return"),
                        new(Type.BRACES_RIGHT, "}")
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
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.FUNCTION, "function"),
                        new(Type.IDENTIFIER, "Test"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.IDENTIFIER, "a"),
                        new(Type.COMMA, ","),
                        new(Type.IDENTIFIER, "b"),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.RETURN, "return"),
                        new(Type.IDENTIFIER, "a"),
                        new(Type.BRACES_RIGHT, "}")
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
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.FUNCTION, "function"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.IDENTIFIER, "a"),
                        new(Type.COMMA, ","),
                        new(Type.IDENTIFIER, "b"),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.RETURN, "return"),
                        new(Type.IDENTIFIER, "a"),
                        new(Type.BRACES_RIGHT, "}")
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

        [Test]
        [TestCaseSource(nameof(FunctionTestCases))]
        public void Parse_Function(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertAst(ParsingTestUtils.Parse(input), expected);
        }
    }
}