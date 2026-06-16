using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.AST;
using Spelllang.Lexer;

namespace Spelllang.Tests.Interpreter
{
    /*
     * TODO: Unify this with other tests
     */
    [TestFixture]
    public class OperatorTests
    {
        private System.IO.TextWriter? _originalOut;

        [SetUp]
        public void SetUp()
        {
            _originalOut = System.Console.Out;
            System.Console.SetOut(new System.IO.StringWriter());
        }

        [TearDown]
        public void TearDown()
        {
            System.Console.SetOut(_originalOut!);
        }

        [Test]
        public void Equal_Ints_SameValue_ReturnsTrue()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "1"),
                new(Type.EQUAL, "=="),
                new(Type.NUMBER, "1")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, true);
        }

        [Test]
        public void Equal_Ints_DifferentValue_ReturnsFalse()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "1"),
                new(Type.EQUAL, "=="),
                new(Type.NUMBER, "2")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, false);
        }

        [Test]
        public void Equal_Floats_SameValue_ReturnsTrue()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "3.14"),
                new(Type.EQUAL, "=="),
                new(Type.NUMBER, "3.14")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, true);
        }

        [Test]
        public void Equal_Floats_DifferentValue_ReturnsFalse()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "3.14"),
                new(Type.EQUAL, "=="),
                new(Type.NUMBER, "2.71")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, false);
        }

        [Test]
        public void Equal_Strings_SameValue_ReturnsTrue()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.STRING, "hello"),
                new(Type.EQUAL, "=="),
                new(Type.STRING, "hello")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, true);
        }

        [Test]
        public void Equal_Strings_DifferentValue_ReturnsFalse()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.STRING, "hello"),
                new(Type.EQUAL, "=="),
                new(Type.STRING, "world")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, false);
        }

        [Test]
        public void Equal_Booleans_SameValue_ReturnsTrue()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.BOOLEAN, "true"),
                new(Type.EQUAL, "=="),
                new(Type.BOOLEAN, "true")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, true);
        }

        [Test]
        public void Equal_Booleans_DifferentValue_ReturnsFalse()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.BOOLEAN, "true"),
                new(Type.EQUAL, "=="),
                new(Type.BOOLEAN, "false")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, false);
        }

        [Test]
        public void Equal_CrossType_ReturnsNull()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "1"),
                new(Type.EQUAL, "=="),
                new(Type.STRING, "1")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeNull(result);
        }

        [Test]
        public void Equal_ComplexExpression_WithAssignment()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.IDENTIFIER, "A"),
                new(Type.ASSIGN, "="),
                new(Type.NUMBER, "10"),
                new(Type.SEMICOLON, ";"),
                new(Type.IDENTIFIER, "A"),
                new(Type.EQUAL, "=="),
                new(Type.NUMBER, "10")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, true);
        }

        [Test]
        public void Equal_ComplexExpression_WithAssignment_Fails()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.IDENTIFIER, "A"),
                new(Type.ASSIGN, "="),
                new(Type.NUMBER, "5"),
                new(Type.SEMICOLON, ";"),
                new(Type.IDENTIFIER, "A"),
                new(Type.EQUAL, "=="),
                new(Type.NUMBER, "10")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, false);
        }

        public static IEnumerable SimpleInfixTestCases
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

        [Test]
        [TestCaseSource(nameof(SimpleInfixTestCases))]
        public void Simple_Integer_Operators(List<Token> input, int expected)
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(input);

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, expected);
        }

        private static TestCaseData BuildSimpleInfixOperatorTestCase(string op, Type operatorType, int expectedResult)
        {
            {
                return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.NUMBER, "3"),
                        new(operatorType, op),
                        new(Type.NUMBER, "3")
                    },
                    expectedResult
                );
            }
        }
    }
}