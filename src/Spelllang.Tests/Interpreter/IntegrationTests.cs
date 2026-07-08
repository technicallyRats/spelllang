using System.Collections.Generic;
using NUnit.Framework;

namespace Spelllang.Tests.Interpreter
{
    [TestFixture]
    public class IntegrationTests
    {
        private System.IO.TextWriter? _originalOut;
        private System.IO.StringWriter? _sw;

        [SetUp]
        public void SetUp()
        {
            _sw = new System.IO.StringWriter();
            _originalOut = System.Console.Out;
            System.Console.SetOut(_sw);
        }

        [TearDown]
        public void TearDown()
        {
            System.Console.SetOut(_originalOut!);
            _sw!.Dispose();
        }

        [Test]
        public void FullPipeline_AssignAndCompare_ReturnsTrue()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter("A = 1; A == 1");

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, true);
        }

        [Test]
        public void FullPipeline_AssignAndCompare_ReturnsFalse()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter("A = 1; A == 2");

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, false);
        }

        [Test]
        public void FullPipeline_PrintBuiltin_WritesToConsole()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(
                "PRINT('hello from integration')",
                new List<(string name, Spelllang.Builtins.IRuntimeBuiltin value)>
                {
                    ("PRINT", new Spelllang.Sample.PrintTestBuiltin())
                }
            );

            interpreter.Run();
            Assert.That(_sw!.ToString().Trim(), Is.EqualTo("String hello from integration"));
        }

        [Test]
        public void FullPipeline_PrintWithVariables()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter(
                "X = 'hello'; PRINT(X)",
                new List<(string name, Spelllang.Builtins.IRuntimeBuiltin value)>
                {
                    ("PRINT", new Spelllang.Sample.PrintTestBuiltin())
                }
            );

            interpreter.Run();
            Assert.That(_sw!.ToString().Trim(), Is.EqualTo("String hello"));
        }

        [Test]
        public void FullPipeline_VariableChaining()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter("A = 1; B = A; B");

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, 1);
        }

        [Test]
        public void FullPipeline_BooleanLiteral_ReturnsBool()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter("true");

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeBoolean(result, true);
        }

        [Test]
        public void FullPipeline_FloatLiteral_ReturnsFloat()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter("3.14");

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeFloat(result, 3.14f);
        }

        [Test]
        public void FullPipeline_StringLiteral_ReturnsString()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter("'hello world'");

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeString(result, "hello world");
        }

        [Test]
        public void FullPipeline_IntegerWithUnderscore_ParsesAndEvaluates()
        {
            var interpreter = InterpreterTestUtils.BuildInterpreter("1_000_000");

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, 1000000);
        }
    }
}
