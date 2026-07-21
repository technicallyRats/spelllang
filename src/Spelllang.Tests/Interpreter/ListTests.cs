using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.Interpreter;
using Spelllang.Lexer;
using Spelllang.Tests.TestUtils;
using Type = Spelllang.Lexer.Type;

namespace Spelllang.Tests.Interpreter
{
    public class ListTests
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
                        new List<IRuntimeVariableBase>
                        {
                            new RuntimeInt(1),
                            new RuntimeInt(2),
                            new RuntimeInt(3)
                        }
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
                        new List<IRuntimeVariableBase>
                        {
                            new RuntimeList(new List<IRuntimeVariableBase>
                            {
                                new RuntimeInt(1),
                                new RuntimeInt(2)
                            }),
                            new RuntimeList(new List<IRuntimeVariableBase>
                            {
                                new RuntimeInt(3)
                            })
                        }
                    ).SetName("Filled lists");
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
                        new List<IRuntimeVariableBase>
                        {
                            new RuntimeList(new List<IRuntimeVariableBase>()),
                            new RuntimeList(new List<IRuntimeVariableBase>())
                        }
                    ).SetName("Empty nested lists");
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
                        new List<IRuntimeVariableBase>
                        {
                            new RuntimeInt(1),
                            new RuntimeInt(2),
                            new RuntimeInt(3)
                        }
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
                        3
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
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("[")),
                            new(Type.BRACKETS_LEFT, "[", inc.Increment("]")),
                            new(Type.NUMBER, "0", inc.Increment("0")),
                            new(Type.BRACKETS_RIGHT, "]", inc.Increment("["))
                        },
                        1
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
                        3
                    ).SetName("Simple index add");
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(ListTestCases))]
        public void Interpret_List(List<Token> input, List<IRuntimeVariableBase> expected)
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(input);

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeList(result, expected);
        }

        [Test]
        [TestCaseSource(nameof(IndexTestCases))]
        public void Interpret_Index(List<Token> input, int expected)
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(input);

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, expected);
        }
    }
}