using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.Lexer;

namespace Spelllang.Tests.Interpreter
{
    public static class InterpreterTestUtils
    {
        public static Spelllang.Interpreter.Interpreter BuildInterpreter(List<Token> tokens)
        {
            var enumerator = new TokenEnumerator(tokens);
            var parser = new Spelllang.Parser.Parser(enumerator);
            return new Spelllang.Interpreter.Interpreter(parser);
        }

        public static Spelllang.Interpreter.Interpreter BuildInterpreter(List<Token> tokens,
            List<(string name, Spelllang.Builtins.IRuntimeBuiltin value)> builtins)
        {
            var enumerator = new TokenEnumerator(tokens);
            var parser = new Spelllang.Parser.Parser(enumerator);
            return new Spelllang.Interpreter.Interpreter(parser, builtins);
        }

        public static Spelllang.Interpreter.Interpreter BuildInterpreter(string source)
        {
            var lexer = new Spelllang.Lexer.Lexer(source);
            var parser = new Spelllang.Parser.Parser(lexer);
            return new Spelllang.Interpreter.Interpreter(parser);
        }

        public static Spelllang.Interpreter.Interpreter BuildInterpreter(string source,
            List<(string name, Spelllang.Builtins.IRuntimeBuiltin value)> builtins)
        {
            var lexer = new Spelllang.Lexer.Lexer(source);
            var parser = new Spelllang.Parser.Parser(lexer);
            return new Spelllang.Interpreter.Interpreter(parser, builtins);
        }

        public static void AssertRuntimeInt(Spelllang.Interpreter.IRuntimeVariableBase actual, int expected)
        {
            Assert.That(actual, Is.InstanceOf<Spelllang.Interpreter.RuntimeInt>());
            Assert.That(((Spelllang.Interpreter.RuntimeInt)actual).GetValue(), Is.EqualTo(expected));
        }

        public static void AssertRuntimeFloat(Spelllang.Interpreter.IRuntimeVariableBase actual, float expected)
        {
            Assert.That(actual, Is.InstanceOf<Spelllang.Interpreter.RuntimeFloat>());
            Assert.That(((Spelllang.Interpreter.RuntimeFloat)actual).GetValue(), Is.EqualTo(expected));
        }

        public static void AssertRuntimeString(Spelllang.Interpreter.IRuntimeVariableBase actual, string expected)
        {
            Assert.That(actual, Is.InstanceOf<Spelllang.Interpreter.RuntimeString>());
            Assert.That(((Spelllang.Interpreter.RuntimeString)actual).GetValue(), Is.EqualTo(expected));
        }

        public static void AssertRuntimeBoolean(Spelllang.Interpreter.IRuntimeVariableBase actual, bool expected)
        {
            Assert.That(actual, Is.InstanceOf<Spelllang.Interpreter.RuntimeBoolean>());
            Assert.That(((Spelllang.Interpreter.RuntimeBoolean)actual).GetValue(), Is.EqualTo(expected));
        }

        public static void AssertRuntimeNull(Spelllang.Interpreter.IRuntimeVariableBase actual)
        {
            Assert.That(actual, Is.InstanceOf<Spelllang.Interpreter.RuntimeNull>());
        }
    }
}
