using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Spelllang.Builtins;
using Spelllang.Lexer;
using Spelllang.Tests.TestBuiltins;
using Spelllang.Tests.TestUtils;
using Type = Spelllang.Lexer.Type;

namespace Spelllang.Tests.Interpreter
{
    [TestFixture]
    public class InterpreterTests
    {
        [SetUp]
        public void SetUp()
        {
            _sw = new StringWriter();
            _originalOut = Console.Out;
            Console.SetOut(_sw);
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetOut(_originalOut!);
            _sw.Dispose();
        }

        private TextWriter? _originalOut;
        private StringWriter? _sw;

        [Test]
        public void EvaluateIntegerLiteral()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "42", inc.Increment("42"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, 42);
        }

        [Test]
        public void EvaluateFloatLiteral()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "3.14", inc.Increment("3.14"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeFloat(result, 3.14f);
        }

        [Test]
        public void EvaluateStringLiteral()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.STRING, "hello", inc.Increment("hello"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeString(result, "hello");
        }

        [Test]
        public void EvaluateTrueBooleanLiteral()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.BOOLEAN, "true", inc.Increment("true"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, true);
        }

        [Test]
        public void EvaluateFalseBooleanLiteral()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.BOOLEAN, "false", inc.Increment("false"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, false);
        }

        [Test]
        public void EvaluateSimpleAssignment()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.IDENTIFIER, "A", inc.Increment("A")),
                new(Type.ASSIGN, "=", inc.Increment("=")),
                new(Type.NUMBER, "1", inc.Increment("1"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, 1);
        }

        [Test]
        public void EvaluateAssignmentThenRetrieval()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.IDENTIFIER, "A", inc.Increment("A")),
                new(Type.ASSIGN, "=", inc.Increment("=")),
                new(Type.NUMBER, "5", inc.Increment("5")),
                new(Type.SEMICOLON, ";", inc.Increment(";")),
                new(Type.IDENTIFIER, "A", inc.Increment("A"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, 5);
        }

        [Test]
        public void EvaluateMultipleAssignments_LastWins()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.IDENTIFIER, "A", inc.Increment("A")),
                new(Type.ASSIGN, "=", inc.Increment("=")),
                new(Type.NUMBER, "1", inc.Increment("1")),
                new(Type.SEMICOLON, ";", inc.Increment(";")),
                new(Type.IDENTIFIER, "A", inc.Increment("A")),
                new(Type.ASSIGN, "=", inc.Increment("=")),
                new(Type.NUMBER, "2", inc.Increment("2")),
                new(Type.SEMICOLON, ";", inc.Increment(";")),
                new(Type.IDENTIFIER, "A", inc.Increment("A"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, 2);
        }

        [Test]
        public void EvaluateStringAssignment()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.IDENTIFIER, "NAME", inc.Increment("NAME")),
                new(Type.ASSIGN, "=", inc.Increment("=")),
                new(Type.STRING, "Spelllang", inc.Increment("Spelllang"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeString(result, "Spelllang");
        }

        [Test]
        public void UnassignedIdentifier_ReturnsError()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.IDENTIFIER, "UNDEFINED_VAR", inc.Increment("UNDEFINED_VAR"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeError(result, "Unknown variable UNDEFINED_VAR");
        }

        [Test]
        public void BuiltinIsCalledWithArguments()
        {
            var builtin = new PrintCapturingBuiltin();
            var builtins = new List<(string name, IRuntimeBuiltin value)>
            {
                ("PRINT", builtin)
            };
            var inc = new Incrementer();

            var interpreter = InterpreterTestUtils.BuildInterpreter(
                new List<Token>
                {
                    new(Type.IDENTIFIER, "PRINT", inc.Increment("PRINT")),
                    new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                    new(Type.STRING, "hello", inc.Increment("hello")),
                    new(Type.COMMA, ",", inc.Increment(",")),
                    new(Type.NUMBER, "42", inc.Increment("42")),
                    new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")"))
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
            var builtin = new PrintCapturingBuiltin();
            var builtins = new List<(string name, IRuntimeBuiltin value)>
            {
                ("PRINT", builtin)
            };
            var inc = new Incrementer();

            var interpreter = InterpreterTestUtils.BuildInterpreter(
                new List<Token>
                {
                    new(Type.IDENTIFIER, "PRINT", inc.Increment("PRINT")),
                    new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                    new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")"))
                },
                builtins
            );

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeNull(result);
            Assert.That(builtin.CallCount, Is.EqualTo(1));
            Assert.That(builtin.LastArgs, Is.Empty);
        }

        [Test]
        public void UnknownIdentifierInCall_ReturnsError()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.IDENTIFIER, "NONEXISTENT", inc.Increment("NONEXISTENT")),
                new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeError(result, "Unexpected resolution for function call: Error");
        }

        [Test]
        public void AssignThenUseInBuiltinCall()
        {
            var builtin = new PrintCapturingBuiltin();
            var builtins = new List<(string name, IRuntimeBuiltin value)>
            {
                ("PRINT", builtin)
            };
            var inc = new Incrementer();

            var interpreter = InterpreterTestUtils.BuildInterpreter(
                new List<Token>
                {
                    new(Type.IDENTIFIER, "X", inc.Increment("X")),
                    new(Type.ASSIGN, "=", inc.Increment("=")),
                    new(Type.NUMBER, "100", inc.Increment("100")),
                    new(Type.SEMICOLON, ";", inc.Increment(";")),
                    new(Type.IDENTIFIER, "PRINT", inc.Increment("PRINT")),
                    new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                    new(Type.IDENTIFIER, "X", inc.Increment("X")),
                    new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")"))
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
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "1_000.5", inc.Increment("1_000.5"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeFloat(result, 1000.5f);
        }

        [Test]
        public void EvaluateIntegerLiteralWithUnderscore()
        {
            var inc = new Incrementer();
            var interpreter = InterpreterTestUtils.BuildInterpreter(new List<Token>
            {
                new(Type.NUMBER, "1_000_000", inc.Increment("1_000_000"))
            });

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, 1000000);
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
                            new(Type.IDENTIFIER, "myBool", inc.Increment("myBool")),
                            new(Type.ASSIGN, "=", inc.Increment("=")),
                            new(Type.BOOLEAN, "true", inc.Increment("true")),
                            new(Type.IF, "if", inc.Increment("if")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.IDENTIFIER, "myBool", inc.Increment("myBool")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}"))
                        },
                        1
                    ).SetName("Simple If");
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.IDENTIFIER, "myBool", inc.Increment("myBool")),
                            new(Type.ASSIGN, "=", inc.Increment("=")),
                            new(Type.BOOLEAN, "false", inc.Increment("false")),
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
                        2
                    ).SetName("Simple If Else");
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.IDENTIFIER, "myBool", inc.Increment("myBool")),
                            new(Type.ASSIGN, "=", inc.Increment("=")),
                            new(Type.BOOLEAN, "false", inc.Increment("false")),
                            new(Type.IDENTIFIER, "myBool2", inc.Increment("myBool2")),
                            new(Type.ASSIGN, "=", inc.Increment("=")),
                            new(Type.BOOLEAN, "true", inc.Increment("true")),
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
                        2
                    ).SetName("If Else If");
                }
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
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.IDENTIFIER, "myInt", inc.Increment("myInt")),
                            new(Type.ASSIGN, "=", inc.Increment("=")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.WHILE, "while", inc.Increment("while")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.IDENTIFIER, "myInt", inc.Increment("myInt")),
                            new(Type.LT, "<", inc.Increment("<")),
                            new(Type.NUMBER, "100", inc.Increment("100")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.IDENTIFIER, "myInt", inc.Increment("myInt")),
                            new(Type.ASSIGN, "=", inc.Increment("=")),
                            new(Type.IDENTIFIER, "myInt", inc.Increment("myInt")),
                            new(Type.PLUS, "+", inc.Increment("+")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}"))
                        },
                        100
                    ).SetName("Simple while");
                }
                {
                    var inc = new Incrementer();
                    yield return new TestCaseData(
                        new List<Token>
                        {
                            new(Type.IDENTIFIER, "myInt", inc.Increment("myInt")),
                            new(Type.ASSIGN, "=", inc.Increment("=")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.WHILE, "while", inc.Increment("while")),
                            new(Type.PARENTHESES_LEFT, "(", inc.Increment("(")),
                            new(Type.IDENTIFIER, "myInt", inc.Increment("myInt")),
                            new(Type.LT, "<", inc.Increment("<")),
                            new(Type.NUMBER, "100", inc.Increment("100")),
                            new(Type.PARENTHESES_RIGHT, ")", inc.Increment(")")),
                            new(Type.BRACES_LEFT, "{", inc.Increment("{")),
                            new(Type.IDENTIFIER, "myInt", inc.Increment("myInt")),
                            new(Type.ASSIGN, "=", inc.Increment("=")),
                            new(Type.IDENTIFIER, "myInt", inc.Increment("myInt")),
                            new(Type.PLUS, "+", inc.Increment("+")),
                            new(Type.NUMBER, "1", inc.Increment("1")),
                            new(Type.BREAK, "break", inc.Increment("break")),
                            new(Type.BRACES_RIGHT, "}", inc.Increment("}")),
                            new(Type.IDENTIFIER, "myInt", inc.Increment("myInt"))
                        },
                        2
                    ).SetName("Simple while with break");
                }
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

        [Test]
        public void Interpreter_ShouldRefuseParserWithErrors()
        {
            // this is nonsense input
            var sourceCode = "/5";
            var lexer = new Spelllang.Lexer.Lexer(sourceCode);
            var parser = new Spelllang.Parser.Parser(lexer);
            var interpreter = new Spelllang.Interpreter.Interpreter(parser);
            parser.DisplayErrors(sourceCode);
            Assert.That(_sw!.ToString().Trim(), Is.EqualTo("No Prefix parser for this token DIVIDE\n/5\n^"));
            var result = interpreter.Run();
            Assert.That(result, Is.Null);
        }
    }
}