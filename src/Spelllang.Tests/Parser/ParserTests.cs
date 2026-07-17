using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.AST;
using Spelllang.Lexer;
using Spelllang.Tests.TestUtils;
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
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.IDENTIFIER, "PRINT", inc.Increment("PRINT")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")"))
                        },
                        ParsingTestUtils.BuildCallProgramNode("PRINT",
                            new List<IExpressionNode>())
                    ).SetName("Simple function call");
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.IDENTIFIER, "PRINT", inc.Increment("PRINT")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.STRING, "1", inc.Increment("1")),
                            new(Type.COMMA, ",", inc.Increment(",")),
                            new(Type.STRING, " + ", inc.Increment(" + ")),
                            new(Type.COMMA, ",", inc.Increment(",")),
                            new(Type.NUMBER, "2", inc.Increment("2")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")"))
                        },
                        ParsingTestUtils.BuildCallProgramNode("PRINT", new List<IExpressionNode>
                        {
                            new StringExpression("1"),
                            new StringExpression(" + "),
                            new IntegerExpression(2)
                        })
                    ).SetName("Simple args function call");
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.IDENTIFIER, "PRINT", inc.Increment("PRINT")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.IDENTIFIER, "GETVAL", inc.Increment("GETVAL")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.NUMBER, "2", inc.Increment("2")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.COMMA, ",", inc.Increment(",")),
                            new(Type.IDENTIFIER, "ABC", inc.Increment("ABC")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")"))
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
        }

        public static IEnumerable IfTestCases
        {
            get
            {
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.IF, "if", inc.Increment("if")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.IDENTIFIER, "myBool", inc.Increment("myBool")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}"))
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
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.IF, "if", inc.Increment("if")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.IDENTIFIER, "myBool", inc.Increment("myBool")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}")),
                            new(Type.ELSE, "else", inc.Increment("else")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.NUMBER, "2", inc.Increment("2")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}"))
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
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.IF, "if", inc.Increment("if")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.IDENTIFIER, "myBool", inc.Increment("myBool")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}")),
                            new(Type.ELSE, "else", inc.Increment("else")),
                            new(Type.IF, "if", inc.Increment("if")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.IDENTIFIER, "myBool2", inc.Increment("myBool2")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.NUMBER, "2", inc.Increment("2")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}"))
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
        }

        public static IEnumerable WhileTestCases
        {
            get
            {
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.WHILE, "while", inc.Increment("while")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.IDENTIFIER, "myBool", inc.Increment("myBool")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}"))
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
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.WHILE, "while", inc.Increment("while")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.IDENTIFIER, "myBool", inc.Increment("myBool")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.BREAK, "break", inc.Increment("break")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}"))
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
        }

        public static IEnumerable ImportTestCases
        {
            get
            {
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.IMPORT, "import", inc.Increment("import")),
                            new(Type.IDENTIFIER, "myLib", inc.Increment("myLib"))
                        },
                        new ProgramNode(new List<IStatementNode>
                        {
                            new ImportStatement(
                                "myLib",
                                ""
                            )
                        })
                    ).SetName("Simple import");
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.IMPORT, "import", inc.Increment("import")),
                            new(Type.IDENTIFIER, "myLib", inc.Increment("myLib")),
                            new(Type.AS, "as", inc.Increment("as")),
                            new(Type.IDENTIFIER, "yourLib", inc.Increment("yourLib"))
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
        }

        [Test]
        [TestCaseSource(nameof(CallTestCases))]
        public void Parse_FunctionCall(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertParserState(ParsingTestUtils.Parse(input), expected);
        }

        [Test]
        [TestCaseSource(nameof(WhileTestCases))]
        public void Parse_While(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertParserState(ParsingTestUtils.Parse(input), expected);
        }

        [Test]
        [TestCaseSource(nameof(IfTestCases))]
        public void Parse_If(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertParserState(ParsingTestUtils.Parse(input), expected);
        }

        [Test]
        [TestCaseSource(nameof(ImportTestCases))]
        public void Parse_Import(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertParserState(ParsingTestUtils.Parse(input), expected);
        }
    }
}