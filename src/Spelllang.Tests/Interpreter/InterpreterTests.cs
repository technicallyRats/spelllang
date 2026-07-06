using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Spelllang.AST;
using Spelllang.Lexer;

namespace Spelllang.Tests.Interpreter
{
    [TestFixture]
    public class InterpreterTests
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
        public void EvaluateIntegerLiteral()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "42")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, 42);
        }

        [Test]
        public void EvaluateFloatLiteral()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "3.14")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeFloat(result, 3.14f);
        }

        [Test]
        public void EvaluateStringLiteral()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.STRING, "hello")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeString(result, "hello");
        }

        [Test]
        public void EvaluateTrueBooleanLiteral()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.BOOLEAN, "true")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, true);
        }

        [Test]
        public void EvaluateFalseBooleanLiteral()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.BOOLEAN, "false")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, false);
        }

        [Test]
        public void EvaluateSimpleAssignment()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.IDENTIFIER, "A"),
                new(Type.ASSIGN, "="),
                new(Type.NUMBER, "1")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, 1);
        }

        [Test]
        public void EvaluateAssignmentThenRetrieval()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.IDENTIFIER, "A"),
                new(Type.ASSIGN, "="),
                new(Type.NUMBER, "5"),
                new(Type.SEMICOLON, ";"),
                new(Type.IDENTIFIER, "A")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, 5);
        }

        [Test]
        public void EvaluateMultipleAssignments_LastWins()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.IDENTIFIER, "A"),
                new(Type.ASSIGN, "="),
                new(Type.NUMBER, "1"),
                new(Type.SEMICOLON, ";"),
                new(Type.IDENTIFIER, "A"),
                new(Type.ASSIGN, "="),
                new(Type.NUMBER, "2"),
                new(Type.SEMICOLON, ";"),
                new(Type.IDENTIFIER, "A")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, 2);
        }

        [Test]
        public void EvaluateStringAssignment()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.IDENTIFIER, "NAME"),
                new(Type.ASSIGN, "="),
                new(Type.STRING, "Spelllang")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeString(result, "Spelllang");
        }

        [Test]
        public void UnassignedIdentifier_ReturnsNull()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.IDENTIFIER, "UNDEFINED_VAR")
            });

            var result = interpreter.Run();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void BuiltinIsCalledWithArguments()
        {
            var builtin = new TestBuiltins.PrintCapturingBuiltin();
            var builtins = new List<(string name, Spelllang.Builtins.IRuntimeBuiltin value)>
            {
                ("PRINT", builtin)
            };

            var interpreter = InterpreterTestUtils.BuildInterpreter(
                new List<Token>
                {
                    new(Type.IDENTIFIER, "PRINT"),
                    new(Type.PARENTHESES_LEFT, "("),
                    new(Type.STRING, "hello"),
                    new(Type.COMMA, ","),
                    new(Type.NUMBER, "42"),
                    new(Type.PARENTHESES_RIGHT, ")")
                },
                builtins
            );

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeNull(result);
            Assert.That(builtin.CallCount, Is.EqualTo(1));
            Assert.That(builtin.LastArgs.Count, Is.EqualTo(2));
            InterpreterTestUtils.AssertRuntimeString(builtin.LastArgs[0], "hello");
            InterpreterTestUtils.AssertRuntimeInt(builtin.LastArgs[1], 42);
        }

        [Test]
        public void BuiltinCall_WithNoArguments()
        {
            var builtin = new TestBuiltins.PrintCapturingBuiltin();
            var builtins = new List<(string name, Spelllang.Builtins.IRuntimeBuiltin value)>
            {
                ("PRINT", builtin)
            };

            var interpreter = InterpreterTestUtils.BuildInterpreter(
                new List<Token>
                {
                    new(Type.IDENTIFIER, "PRINT"),
                    new(Type.PARENTHESES_LEFT, "("),
                    new(Type.PARENTHESES_RIGHT, ")")
                },
                builtins
            );

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeNull(result);
            Assert.That(builtin.CallCount, Is.EqualTo(1));
            Assert.That(builtin.LastArgs, Is.Empty);
        }

        [Test]
        public void UnknownIdentifierInCall_ReturnsNull()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.IDENTIFIER, "NONEXISTENT"),
                new(Type.PARENTHESES_LEFT, "("),
                new(Type.PARENTHESES_RIGHT, ")")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeNull(result);
        }

        [Test]
        public void AssignThenUseInBuiltinCall()
        {
            var builtin = new TestBuiltins.PrintCapturingBuiltin();
            var builtins = new List<(string name, Spelllang.Builtins.IRuntimeBuiltin value)>
            {
                ("PRINT", builtin)
            };

            var interpreter = InterpreterTestUtils.BuildInterpreter(
                new List<Token>
                {
                    new(Type.IDENTIFIER, "X"),
                    new(Type.ASSIGN, "="),
                    new(Type.NUMBER, "100"),
                    new(Type.SEMICOLON, ";"),
                    new(Type.IDENTIFIER, "PRINT"),
                    new(Type.PARENTHESES_LEFT, "("),
                    new(Type.IDENTIFIER, "X"),
                    new(Type.PARENTHESES_RIGHT, ")")
                },
                builtins
            );

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeNull(result);
            Assert.That(builtin.CallCount, Is.EqualTo(1));
            Assert.That(builtin.LastArgs.Count, Is.EqualTo(1));
            InterpreterTestUtils.AssertRuntimeInt(builtin.LastArgs[0], 100);
        }

        [Test]
        public void EvaluateFloatLiteralWithUnderscore()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "1_000.5")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeFloat(result, 1000.5f);
        }

        [Test]
        public void EvaluateIntegerLiteralWithUnderscore()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "1_000_000")
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, 1000000);
        }

        public static IEnumerable IfTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.IDENTIFIER, "myBool"),
                        new(Type.ASSIGN, "="),
                        new(Type.BOOLEAN, "true"),
                        new(Type.IF, "if"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.IDENTIFIER, "myBool"),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.NUMBER, "1"),
                        new(Type.BRACES_RIGHT, "}")
                    },
                    1
                ).SetName("Simple If");
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.IDENTIFIER, "myBool"),
                        new(Type.ASSIGN, "="),
                        new(Type.BOOLEAN, "false"),
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
                    2
                ).SetName("Simple If Else");
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.IDENTIFIER, "myBool"),
                        new(Type.ASSIGN, "="),
                        new(Type.BOOLEAN, "false"),
                        new(Type.IDENTIFIER, "myBool2"),
                        new(Type.ASSIGN, "="),
                        new(Type.BOOLEAN, "true"),
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
                    2
                ).SetName("If Else If");
            }
        }

        [Test]
        [TestCaseSource(nameof(IfTestCases))]
        public void Interpret_If(List<Token> input, int expected)
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(input);

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, expected);
        }

        public static IEnumerable WhileTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.IDENTIFIER, "myInt"),
                        new(Type.ASSIGN, "="),
                        new(Type.NUMBER, "1"),
                        new(Type.WHILE, "while"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.IDENTIFIER, "myInt"),
                        new(Type.LT, "<"),
                        new(Type.NUMBER, "100"),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.IDENTIFIER, "myInt"),
                        new(Type.ASSIGN, "="),
                        new(Type.IDENTIFIER, "myInt"),
                        new(Type.PLUS, "+"),
                        new(Type.NUMBER, "1"),
                        new(Type.BRACES_RIGHT, "}")
                    },
                    100
                ).SetName("Simple while");
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.IDENTIFIER, "myInt"),
                        new(Type.ASSIGN, "="),
                        new(Type.NUMBER, "1"),
                        new(Type.WHILE, "while"),
                        new(Type.PARENTHESES_LEFT, "("),
                        new(Type.IDENTIFIER, "myInt"),
                        new(Type.LT, "<"),
                        new(Type.NUMBER, "100"),
                        new(Type.PARENTHESES_RIGHT, ")"),
                        new(Type.BRACES_LEFT, "{"),
                        new(Type.IDENTIFIER, "myInt"),
                        new(Type.ASSIGN, "="),
                        new(Type.IDENTIFIER, "myInt"),
                        new(Type.PLUS, "+"),
                        new(Type.NUMBER, "1"),
                        new(Type.BREAK, "break"),
                        new(Type.BRACES_RIGHT, "}"),
                        new(Type.IDENTIFIER, "myInt")
                    },
                    2
                ).SetName("Simple while with break");
            }
        }

        [Test]
        [TestCaseSource(nameof(WhileTestCases))]
        public void Interpret_While(List<Token> input, int expected)
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(input);

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, expected);
        }
    }
}