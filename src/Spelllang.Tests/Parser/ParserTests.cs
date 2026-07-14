using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.AST;
using Spelllang.Lexer;
using Type = Spelllang.Lexer.Type;

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

        public static IEnumerable IfTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.IF, "if"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.IDENTIFIER, "myBool"),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.NUMBER, "1"),
                        new(Type.BRACES_RIGHT, "}")
                    },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new IfStatement(
                            new ProgramNode(new List<IStatementNode>
                                { new ExpressionStatement(new IntegerExpression(1)) }),
                            new ProgramNode(),
                            new IdentifierExpression("myBool")
                        )
                    })
                ).SetName("Simple If");
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.IF, "if"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.IDENTIFIER, "myBool"),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.NUMBER, "1"),
                        new(Type.BRACES_RIGHT, "}"),
                        new(Type.ELSE, "else"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.NUMBER, "2"),
                        new(Type.BRACES_RIGHT, "}")
                    },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new IfStatement(
                            new ProgramNode(new List<IStatementNode>
                                { new ExpressionStatement(new IntegerExpression(1)) }),
                            new ProgramNode(new List<IStatementNode>
                                { new ExpressionStatement(new IntegerExpression(2)) }),
                            new IdentifierExpression("myBool")
                        )
                    })
                ).SetName("Simple If Else");
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.IF, "if"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.IDENTIFIER, "myBool"),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.NUMBER, "1"),
                        new(Type.BRACES_RIGHT, "}"),
                        new(Type.ELSE, "else"),
                        new(Type.IF, "if"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.IDENTIFIER, "myBool2"),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.NUMBER, "2"),
                        new(Type.BRACES_RIGHT, "}")
                    },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new IfStatement(
                            new ProgramNode(new List<IStatementNode>
                                { new ExpressionStatement(new IntegerExpression(1)) }),
                            new ProgramNode(new List<IStatementNode>
                            {
                                new IfStatement(
                                    new ProgramNode(new List<IStatementNode>
                                    {
                                        new ExpressionStatement(new IntegerExpression(2))
                                    }),
                                    new ProgramNode(),
                                    new IdentifierExpression("myBool2")
                                )
                            }),
                            new IdentifierExpression("myBool")
                        )
                    })
                ).SetName("If Else If");
            }
        }

        public static IEnumerable WhileTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.WHILE, "while"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.IDENTIFIER, "myBool"),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.NUMBER, "1"),
                        new(Type.BRACES_RIGHT, "}")
                    },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new WhileStatement(
                            new ProgramNode(new List<IStatementNode>
                                { new ExpressionStatement(new IntegerExpression(1)) }),
                            new IdentifierExpression("myBool")
                        )
                    })
                ).SetName("Simple While");
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.WHILE, "while"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.IDENTIFIER, "myBool"),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.NUMBER, "1"),
                        new(Type.BREAK, "break"),
                        new(Type.BRACES_RIGHT, "}")
                    },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new WhileStatement(
                            new ProgramNode(new List<IStatementNode>
                            {
                                new ExpressionStatement(new IntegerExpression(1)),
                                new BreakStatement()
                            }),
                            new IdentifierExpression("myBool")
                        )
                    })
                ).SetName("Simple While with break");
            }
        }

        public static IEnumerable ImportTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.IMPORT, "import"),
                        new(Type.IDENTIFIER, "myLib")
                    },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new ImportStatement(
                            "myLib",
                            ""
                        )
                    })
                ).SetName("Simple import");
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.IMPORT, "import"),
                        new(Type.IDENTIFIER, "myLib"),
                        new(Type.AS, "as"),
                        new(Type.IDENTIFIER, "yourLib")
                    },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new ImportStatement(
                            "myLib",
                            "yourLib"
                        )
                    })
                ).SetName("Named import");
            }
        }

        [Test]
        [TestCaseSource(nameof(CallTestCases))]
        public void Parse_FunctionCall(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertAst(ParsingTestUtils.Parse(input), expected);
        }

        [Test]
        [TestCaseSource(nameof(WhileTestCases))]
        public void Parse_While(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertAst(ParsingTestUtils.Parse(input), expected);
        }

        [Test]
        [TestCaseSource(nameof(IfTestCases))]
        public void Parse_If(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertAst(ParsingTestUtils.Parse(input), expected);
        }

        [Test]
        [TestCaseSource(nameof(ImportTestCases))]
        public void Parse_Import(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertAst(ParsingTestUtils.Parse(input), expected);
        }
    }
}