using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Spelllang.Builtins;
using Spelllang.Interpreter;

namespace Spelllang.Tests.Builtins
{
    [TestFixture]
    public class BuiltinTests
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
        public void PrintBuiltin_Call_ReturnsRuntimeNull()
        {
            var builtin = new PrintBuiltin();

            var result = builtin.Call(new List<IRuntimeVariableBase>());

            Assert.That(result, Is.InstanceOf<RuntimeNull>());
        }

        [Test]
        public void PrintBuiltin_Call_WithSingleArg_WritesToConsole()
        {
            var builtin = new PrintBuiltin();

            builtin.Call(new List<IRuntimeVariableBase>
            {
                new RuntimeString("hello world")
            });

            Assert.That(_sw!.ToString().Trim(), Is.EqualTo("String hello world"));
        }

        [Test]
        public void PrintBuiltin_Call_WithMultipleArgs_WritesConcatenatedToConsole()
        {
            var builtin = new PrintBuiltin();

            builtin.Call(new List<IRuntimeVariableBase>
            {
                new RuntimeString("hello"),
                new RuntimeString(" "),
                new RuntimeString("world")
            });

            Assert.That(_sw!.ToString().Trim(), Is.EqualTo("String helloString  String world"));
        }

        [Test]
        public void PrintBuiltin_Call_WithNoArgs_WritesEmptyLine()
        {
            var builtin = new PrintBuiltin();

            builtin.Call(new List<IRuntimeVariableBase>());

            Assert.That(_sw!.ToString(), Is.EqualTo(System.Environment.NewLine));
        }

        [Test]
        public void PrintBuiltin_Call_WithMixedTypes_ConcatenatesStrings()
        {
            var builtin = new PrintBuiltin();

            builtin.Call(new List<IRuntimeVariableBase>
            {
                new RuntimeInt(42),
                new RuntimeString(" is the answer"),
                new RuntimeBoolean(true)
            });

            Assert.That(_sw!.ToString().Trim(), Is.EqualTo("Int 42String  is the answerBoolean True"));
        }

        [Test]
        public void PrintBuiltin_CheckParamtypes_ReturnsFalse()
        {
            var builtin = new PrintBuiltin();

            Assert.That(builtin.CheckParamtypes(), Is.False);
        }

        [Test]
        public void BridgeValidator_NoCheck_AlwaysPasses()
        {
            var builtin = new PrintBuiltin();

            var result = BridgeValidator.ParamsMatchExpectedTypes(
                builtin,
                new List<IRuntimeVariableBase> { new RuntimeInt(1), new RuntimeString("a") }
            );

            Assert.That(result, Is.True);
        }
    }
}
