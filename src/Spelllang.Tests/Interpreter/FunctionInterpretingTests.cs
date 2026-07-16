using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.Lexer;
using Spelllang.Tests.TestUtils;

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
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.FUNCTION, "function", inc.Increment("function")),
                            new(Type.IDENTIFIER, "add", inc.Increment("add")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.IDENTIFIER, "a", inc.Increment("a")),
                            new(Type.COMMA, ",", inc.Increment(",")),
                            new(Type.IDENTIFIER, "b", inc.Increment("b")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.RETURN, "return", inc.Increment("return")),
                            new(Type.IDENTIFIER, "a", inc.Increment("a")),
                            new(Type.PLUS, "+", inc.Increment("+")),
                            new(Type.IDENTIFIER, "b", inc.Increment("b")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}")),
                            new(Type.IDENTIFIER, "add", inc.Increment("add")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.NUMBER, "2", inc.Increment("2")),
                            new(Type.COMMA, ",", inc.Increment(",")),
                            new(Type.NUMBER, "3", inc.Increment("3")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")"))
                        },
                        5
                    ).SetName("Call Function with arguments and return value");
                }

                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.FUNCTION, "function", inc.Increment("function")),
                            new(Type.IDENTIFIER, "abc", inc.Increment("abc")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.RETURN, "return", inc.Increment("return")),
                            new(Type.NUMBER, "2", inc.Increment("2")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}")),
                            new(Type.IDENTIFIER, "abc", inc.Increment("abc")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")"))
                        },
                        2
                    ).SetName("Call Function without arguments but with return value");
                }
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