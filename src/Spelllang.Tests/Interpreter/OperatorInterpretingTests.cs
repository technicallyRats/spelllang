using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Spelllang.Lexer;
using Spelllang.Tests.TestUtils;
using Type = Spelllang.Lexer.Type;

namespace Spelllang.Tests.Interpreter
{
    /*
     * TODO: Unify this with other tests
     */
    [TestFixture]
    public class OperatorInterpretingTests
    {
        [SetUp]
        public void SetUp()
        {
            _originalOut = Console.Out;
            Console.SetOut(new StringWriter());
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetOut(_originalOut!);
        }

        private TextWriter? _originalOut;

        [Test]
        public void Equal_Ints_SameValue_ReturnsTrue()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "1", inc.Increment("1")),
                new(Type.EQUAL, "==", inc.Increment("==")),
                new(Type.NUMBER, "1", inc.Increment("1"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, true);
        }

        [Test]
        public void Equal_Ints_DifferentValue_ReturnsFalse()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "1", inc.Increment("1")),
                new(Type.EQUAL, "==", inc.Increment("==")),
                new(Type.NUMBER, "2", inc.Increment("2"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, false);
        }

        [Test]
        public void Equal_Floats_SameValue_ReturnsTrue()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "3.14", inc.Increment("3.14")),
                new(Type.EQUAL, "==", inc.Increment("==")),
                new(Type.NUMBER, "3.14", inc.Increment("3.14"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, true);
        }

        [Test]
        public void Equal_Floats_DifferentValue_ReturnsFalse()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "3.14", inc.Increment("3.14")),
                new(Type.EQUAL, "==", inc.Increment("==")),
                new(Type.NUMBER, "2.71", inc.Increment("2.71"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, false);
        }

        [Test]
        public void Equal_Strings_SameValue_ReturnsTrue()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.STRING, "hello", inc.Increment("hello")),
                new(Type.EQUAL, "==", inc.Increment("==")),
                new(Type.STRING, "hello", inc.Increment("hello"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, true);
        }

        [Test]
        public void Equal_Strings_DifferentValue_ReturnsFalse()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.STRING, "hello", inc.Increment("hello")),
                new(Type.EQUAL, "==", inc.Increment("==")),
                new(Type.STRING, "world", inc.Increment("world"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, false);
        }

        [Test]
        public void Equal_Booleans_SameValue_ReturnsTrue()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.BOOLEAN, "true", inc.Increment("true")),
                new(Type.EQUAL, "==", inc.Increment("==")),
                new(Type.BOOLEAN, "true", inc.Increment("true"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, true);
        }

        [Test]
        public void Equal_Booleans_DifferentValue_ReturnsFalse()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.BOOLEAN, "true", inc.Increment("true")),
                new(Type.EQUAL, "==", inc.Increment("==")),
                new(Type.BOOLEAN, "false", inc.Increment("false"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, false);
        }

        [Test]
        public void Equal_CrossType_ReturnsNull()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "1", inc.Increment("1")),
                new(Type.EQUAL, "==", inc.Increment("==")),
                new(Type.STRING, "1", inc.Increment("1"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeNull(result);
        }

        [Test]
        public void Equal_ComplexExpression_WithAssignment()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.IDENTIFIER, "A", inc.Increment("A")),
                new(Type.ASSIGN, "=", inc.Increment("=")),
                new(Type.NUMBER, "10", inc.Increment("10")),
                new(Type.SEMICOLON, ";", inc.Increment(";")),
                new(Type.IDENTIFIER, "A", inc.Increment("A")),
                new(Type.EQUAL, "==", inc.Increment("==")),
                new(Type.NUMBER, "10", inc.Increment("10"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, true);
        }

        [Test]
        public void Equal_ComplexExpression_WithAssignment_Fails()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.IDENTIFIER, "A", inc.Increment("A")),
                new(Type.ASSIGN, "=", inc.Increment("=")),
                new(Type.NUMBER, "5", inc.Increment("5")),
                new(Type.SEMICOLON, ";", inc.Increment(";")),
                new(Type.IDENTIFIER, "A", inc.Increment("A")),
                new(Type.EQUAL, "==", inc.Increment("==")),
                new(Type.NUMBER, "10", inc.Increment("10"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, false);
        }

        public static IEnumerable SimpleInfixIntegerTestCases
        {
            get
            {
                yield return BuildSimpleInfixOperatorTestCase("+", Type.PLUS, 6).SetName("Simple plus");
                yield return BuildSimpleInfixOperatorTestCase("-", Type.MINUS, 0).SetName("Simple minus");
                yield return BuildSimpleInfixOperatorTestCase("*", Type.MULTIPLY, 9).SetName("Simple multiply");
                yield return BuildSimpleInfixOperatorTestCase("/", Type.DIVIDE, 1).SetName("Simple divide");
                yield return BuildSimpleInfixOperatorTestCase("%", Type.MODULO, 0).SetName("Simple modulo");
            }
        }

        public static IEnumerable SimpleInfixIntegerToBooleanTestCases
        {
            get
            {
                yield return BuildSimpleInfixIntegerToBooleanOperatorTestCase(">", "3", "1", Type.GT, true)
                    .SetName("Simple GT truthy");
                yield return BuildSimpleInfixIntegerToBooleanOperatorTestCase(">", "1", "3", Type.GT, false)
                    .SetName("Simple GT falsy");
                yield return BuildSimpleInfixIntegerToBooleanOperatorTestCase(">=", "3", "3", Type.GTE, true)
                    .SetName("Simple GTE truthy eq");
                yield return BuildSimpleInfixIntegerToBooleanOperatorTestCase(">=", "3", "1", Type.GTE, true)
                    .SetName("Simple GTE truthy gt");
                yield return BuildSimpleInfixIntegerToBooleanOperatorTestCase(">=", "1", "3", Type.GTE, false)
                    .SetName("Simple GTE falsy");
                yield return BuildSimpleInfixIntegerToBooleanOperatorTestCase("<", "1", "3", Type.LT, true)
                    .SetName("Simple LT truthy");
                yield return BuildSimpleInfixIntegerToBooleanOperatorTestCase("<", "3", "1", Type.LT, false)
                    .SetName("Simple LT falsy");
                yield return BuildSimpleInfixIntegerToBooleanOperatorTestCase("<=", "3", "3", Type.LTE, true)
                    .SetName("Simple LTE truthy eq");
                yield return BuildSimpleInfixIntegerToBooleanOperatorTestCase("<=", "1", "3", Type.LTE, true)
                    .SetName("Simple LTE truthy gt");
                yield return BuildSimpleInfixIntegerToBooleanOperatorTestCase("<=", "3", "1", Type.LTE, false)
                    .SetName("Simple LTE falsy");
                yield return BuildSimpleInfixIntegerToBooleanOperatorTestCase("==", "1", "1", Type.EQUAL, true)
                    .SetName("Simple EQ truthy");
                yield return BuildSimpleInfixIntegerToBooleanOperatorTestCase("==", "3", "1", Type.EQUAL, false)
                    .SetName("Simple EQ falsy");
                yield return BuildSimpleInfixIntegerToBooleanOperatorTestCase("!=", "3", "1", Type.NOT_EQUAL, true)
                    .SetName("Simple NEQ truthy");
                yield return BuildSimpleInfixIntegerToBooleanOperatorTestCase("!=", "1", "1", Type.NOT_EQUAL, false)
                    .SetName("Simple NEQ falsy");
            }
        }

        public static IEnumerable SimpleInfixBooleanTestCases
        {
            get
            {
                yield return BuildSimpleInfixBooleanOperatorTestCase("&&", "true", "true", Type.AND, true)
                    .SetName("Simple AND truthy");
                yield return BuildSimpleInfixBooleanOperatorTestCase("&&", "false", "true", Type.AND, false)
                    .SetName("Simple AND falsy");
                yield return BuildSimpleInfixBooleanOperatorTestCase("&&", "true", "false", Type.AND, false)
                    .SetName("Simple AND falsy (reverse)");
                yield return BuildSimpleInfixBooleanOperatorTestCase("&&", "false", "false", Type.AND, false)
                    .SetName("Simple AND falsy (total)");
                yield return BuildSimpleInfixBooleanOperatorTestCase("||", "true", "true", Type.OR, true)
                    .SetName("Simple OR truthy");
                yield return BuildSimpleInfixBooleanOperatorTestCase("||", "false", "true", Type.OR, true)
                    .SetName("Simple OR truthy (A)");
                yield return BuildSimpleInfixBooleanOperatorTestCase("||", "true", "false", Type.OR, true)
                    .SetName("Simple OR truthy (B)");
                yield return BuildSimpleInfixBooleanOperatorTestCase("||", "false", "false", Type.OR, false)
                    .SetName("Simple OR falsy");
            }
        }

        public static IEnumerable SimpleIntegerPrefixTestCases
        {
            get
            {
                yield return BuildSimpleIntegerPrefixOperatorTestCase("+", Type.PLUS, 3).SetName("Plus prefix");
                yield return BuildSimpleIntegerPrefixOperatorTestCase("-", Type.MINUS, -3).SetName("Minus prefix");
            }
        }

        public static IEnumerable SimpleBooleanPrefixTestCases
        {
            get
            {
                var inc = new Incrementer();
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.NOT, "!", inc.Increment(" ")),
                        new(Type.BOOLEAN, "true", inc.Increment("true"))
                    },
                    false
                ).SetName("Not prefix true");
                inc.Reset();
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.NOT, "!", inc.Increment(" ")),
                        new(Type.BOOLEAN, "false", inc.Increment("false"))
                    },
                    true
                ).SetName("Not prefix false");
            }
        }

        [Test]
        [TestCaseSource(nameof(SimpleInfixIntegerTestCases))]
        public void Simple_Infix_Integer_Operators(List<Token> input, int expected)
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(input);

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, expected);
        }

        [Test]
        [TestCaseSource(nameof(SimpleInfixIntegerToBooleanTestCases))]
        public void Simple_Infix_Integer_To_Boolean_Operators(List<Token> input, bool expected)
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(input);

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, expected);
        }

        [Test]
        [TestCaseSource(nameof(SimpleInfixBooleanTestCases))]
        public void Simple_Infix_Boolean_Operators(List<Token> input, bool expected)
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(input);

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, expected);
        }

        [Test]
        [TestCaseSource(nameof(SimpleIntegerPrefixTestCases))]
        public void Simple_Integer_Prefix_Operators(List<Token> input, int expected)
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(input);

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, expected);
        }

        [Test]
        [TestCaseSource(nameof(SimpleBooleanPrefixTestCases))]
        public void Simple_Boolean_Prefix_Operators(List<Token> input, bool expected)
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(input);

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, expected);
        }

        private static TestCaseData BuildSimpleInfixOperatorTestCase(string op, Type operatorType, int expectedResult)
        {
            {
                var inc = new Incrementer();
                return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.NUMBER, "3", inc.Increment("3")),
                        new(operatorType, op, inc.Increment(op)),
                        new(Type.NUMBER, "3", inc.Increment("3"))
                    },
                    expectedResult
                );
            }
        }

        private static TestCaseData BuildSimpleInfixIntegerToBooleanOperatorTestCase(string op, string numberA,
            string numberB,
            Type operatorType,
            bool expectedResult)
        {
            {
                return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.NUMBER, numberA, 0),
                        new(operatorType, op, numberA.Length),
                        new(Type.NUMBER, numberB, numberA.Length + op.Length)
                    },
                    expectedResult
                );
            }
        }

        private static TestCaseData BuildSimpleInfixBooleanOperatorTestCase(string op, string boolA,
            string boolB,
            Type operatorType,
            bool expectedResult)
        {
            {
                return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.BOOLEAN, boolA, 0),
                        new(operatorType, op, 4),
                        new(Type.BOOLEAN, boolB, 4 + op.Length)
                    },
                    expectedResult
                );
            }
        }

        private static TestCaseData BuildSimpleIntegerPrefixOperatorTestCase(string op, Type operatorType,
            int expectedResult)
        {
            {
                var inc = new Incrementer();
                return new TestCaseData(
                    new List<Token>
                    {
                        new(operatorType, op, inc.Increment(op)),
                        new(Type.NUMBER, "3", inc.Increment("3"))
                    },
                    expectedResult
                );
            }
        }
    }
}