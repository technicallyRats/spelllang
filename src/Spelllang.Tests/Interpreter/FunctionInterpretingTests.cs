using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.Lexer;

namespace Spelllang.Tests.Interpreter
{
    /*
     * TODO: Unify this with other tests
     */
    [TestFixture]
    public class FunctionInterpretingTests
    {
        public static IEnumerable FunctionTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.FUNCTION, "function"),
                        new(Type.IDENTIFIER, "add"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.IDENTIFIER, "a"),
                        new(Type.COMMA, ","),
                        new(Type.IDENTIFIER, "b"),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.RETURN, "return"),
                        new(Type.IDENTIFIER, "a"),
                        new(Type.PLUS, "+"),
                        new(Type.IDENTIFIER, "b"),
                        new(Type.BRACES_RIGHT, "}"),
                        new(Type.IDENTIFIER, "add"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.NUMBER, "2"),
                        new(Type.COMMA, ","),
                        new(Type.NUMBER, "3"),
                        new(Type.PARENTHESES_RIGHT, ")")
                    },
                    5
                ).SetName("Call Function with arguments and return value");

                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.FUNCTION, "function"),
                        new(Type.IDENTIFIER, "abc"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.RETURN, "return"),
                        new(Type.NUMBER, "2"),
                        new(Type.BRACES_RIGHT, "}"),
                        new(Type.IDENTIFIER, "abc"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.PARENTHESES_RIGHT, ")")
                    },
                    2
                ).SetName("Call Function without arguments but with return value");
            }
        }

        [Test]
        [TestCaseSource(nameof(FunctionTestCases))]
        public void Interpret_Function(List<Token> input, int expected)
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(input);

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, expected);
        }
    }
}