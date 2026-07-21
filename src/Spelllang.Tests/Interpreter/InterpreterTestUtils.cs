using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.Builtins;
using Spelllang.Interpreter;
using Spelllang.Interpreter.Importer;
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
            List<(string name, IRuntimeBuiltin value)> builtins)
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
            List<(string name, IRuntimeBuiltin value)> builtins)
        {
            var lexer = new Spelllang.Lexer.Lexer(source);
            var parser = new Spelllang.Parser.Parser(lexer);
            return new Spelllang.Interpreter.Interpreter(parser, builtins);
        }

        public static Spelllang.Interpreter.Interpreter BuildInterpreter(string source,
            List<(string name, IRuntimeBuiltin value)> builtins,
            List<IImportResolver> resolvers
        )
        {
            var lexer = new Spelllang.Lexer.Lexer(source);
            var parser = new Spelllang.Parser.Parser(lexer);
            return new Spelllang.Interpreter.Interpreter(parser, builtins, resolvers);
        }

        public static void AssertRuntimeInt(IRuntimeVariableBase actual, int expected)
        {
            Assert.That(actual, Is.InstanceOf<RuntimeInt>());
            Assert.That(((RuntimeInt)actual).GetValue(), Is.EqualTo(expected));
        }

        public static void AssertRuntimeFloat(IRuntimeVariableBase actual, float expected)
        {
            Assert.That(actual, Is.InstanceOf<RuntimeFloat>());
            Assert.That(((RuntimeFloat)actual).GetValue(), Is.EqualTo(expected));
        }

        public static void AssertRuntimeString(IRuntimeVariableBase actual, string expected)
        {
            Assert.That(actual, Is.InstanceOf<RuntimeString>());
            Assert.That(((RuntimeString)actual).GetValue(), Is.EqualTo(expected));
        }

        public static void AssertRuntimeBoolean(IRuntimeVariableBase actual, bool expected)
        {
            Assert.That(actual, Is.InstanceOf<RuntimeBoolean>());
            Assert.That(((RuntimeBoolean)actual).GetValue(), Is.EqualTo(expected));
        }

        public static void AssertRuntimeNull(IRuntimeVariableBase actual)
        {
            Assert.That(actual, Is.InstanceOf<RuntimeNull>());
        }

        public static void AssertRuntimeError(IRuntimeVariableBase actual, string expected)
        {
            Assert.That(actual, Is.InstanceOf<RuntimeError>());
            Assert.That(((RuntimeError)actual).GetReason(), Is.EqualTo(expected));
        }
    }
}