using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.AST;
using Spelllang.Lexer;
using Spelllang.Tests.TestUtils;

namespace Spelllang.Tests.Parser
{
    public class ListParsingTests
    {
        public static IEnumerable ListTestCases
        {
            get
            {
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.COMMA, ",", inc.Increment(",")),
                            new(Type.NUMBER, "2", inc.Increment("2")),
                            new(Type.COMMA, ",", inc.Increment(",")),
                            new(Type.NUMBER, "3", inc.Increment("3")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("["))
                        },
                        new ProgramNode(new List<IStatementNode>
                        {
                            new ExpressionStatement(
                                new ListExpression(new List<IExpressionNode>
                                {
                                    new IntegerExpression(1),
                                    new IntegerExpression(2),
                                    new IntegerExpression(3)
                                })
                            )
                        })
                    ).SetName("Simple list");
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.COMMA, ",", inc.Increment(",")),
                            new(Type.NUMBER, "2", inc.Increment("2")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("[")),
                            new(Type.COMMA, ",", inc.Increment(",")),
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.NUMBER, "3", inc.Increment("3")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("[")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("["))
                        },
                        new ProgramNode(new List<IStatementNode>
                        {
                            new ExpressionStatement(
                                new ListExpression(new List<IExpressionNode>
                                {
                                    new ListExpression(new List<IExpressionNode>
                                    {
                                        new IntegerExpression(1),
                                        new IntegerExpression(2)
                                    }),
                                    new ListExpression(new List<IExpressionNode>
                                    {
                                        new IntegerExpression(3)
                                    })
                                })
                            )
                        })
                    ).SetName("Empty nested lists");
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("[")),
                            new(Type.COMMA, ",", inc.Increment(",")),
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("[")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("["))
                        },
                        new ProgramNode(new List<IStatementNode>
                        {
                            new ExpressionStatement(
                                new ListExpression(new List<IExpressionNode>
                                {
                                    new ListExpression(new List<IExpressionNode>()),
                                    new ListExpression(new List<IExpressionNode>())
                                })
                            )
                        })
                    ).SetName("Filled nested lists");
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.COMMA, ",", inc.Increment(",")),
                            new(Type.NUMBER, "2", inc.Increment("2")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("[")),
                            new(Type.PLUS, "+", inc.Increment("+")),
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.NUMBER, "3", inc.Increment("3")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("["))
                        },
                        new ProgramNode(new List<IStatementNode>
                        {
                            new ExpressionStatement(
                                new InfixExpression(
                                    new ListExpression(new List<IExpressionNode>
                                    {
                                        new IntegerExpression(1),
                                        new IntegerExpression(2)
                                    }),
                                    new ListExpression(new List<IExpressionNode>
                                    {
                                        new IntegerExpression(3)
                                    }),
                                    "+"
                                )
                            )
                        })
                    ).SetName("Simple list add");
                }
            }
        }

        public static IEnumerable IndexTestCases
        {
            get
            {
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.COMMA, ",", inc.Increment(",")),
                            new(Type.NUMBER, "2", inc.Increment("2")),
                            new(Type.COMMA, ",", inc.Increment(",")),
                            new(Type.NUMBER, "3", inc.Increment("3")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("[")),
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.NUMBER, "2", inc.Increment("2")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("["))
                        },
                        new ProgramNode(new List<IStatementNode>
                        {
                            new ExpressionStatement(
                                new IndexExpression(
                                    new ListExpression(new List<IExpressionNode>
                                    {
                                        new IntegerExpression(1),
                                        new IntegerExpression(2),
                                        new IntegerExpression(3)
                                    }),
                                    new IntegerExpression(2)
                                )
                            )
                        })
                    ).SetName("Simple index");
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("[")),
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.NUMBER, "0", inc.Increment("0")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("[")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("["))
                        },
                        new ProgramNode(new List<IStatementNode>
                        {
                            new ExpressionStatement(
                                new ListExpression(new List<IExpressionNode>
                                {
                                    new IndexExpression(
                                        new ListExpression(new List<IExpressionNode>
                                            {
                                                new IntegerExpression(1)
                                            }
                                        ),
                                        new IntegerExpression(0)
                                    )
                                })
                            )
                        })
                    ).SetName("Nested index");
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("[")),
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.NUMBER, "0", inc.Increment("0")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("[")),
                            new(Type.PLUS, "+", inc.Increment("+")),
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.NUMBER, "2", inc.Increment("2")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("[")),
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.NUMBER, "0", inc.Increment("0")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("["))
                        },
                        new ProgramNode(new List<IStatementNode>
                        {
                            new ExpressionStatement(
                                new InfixExpression(
                                    new IndexExpression(
                                        new ListExpression(new List<IExpressionNode>
                                        {
                                            new IntegerExpression(1)
                                        }),
                                        new IntegerExpression(0)
                                    ),
                                    new IndexExpression(
                                        new ListExpression(new List<IExpressionNode>
                                        {
                                            new IntegerExpression(2)
                                        }),
                                        new IntegerExpression(0)
                                    ),
                                    "+"
                                )
                            )
                        })
                    ).SetName("Simple index");
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(ListTestCases))]
        public void Parse_List(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertParserState(ParsingTestUtils.Parse(input), expected);
        }

        [Test]
        [TestCaseSource(nameof(IndexTestCases))]
        public void Parse_Index(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertParserState(ParsingTestUtils.Parse(input), expected);
        }
    }
}