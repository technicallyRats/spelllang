using System.Collections.Generic;
using Spelllang.Lexer;
using NUnit.Framework;

namespace Spelllang.Tests.Lexer
{
    [TestFixture]
    public class LexerTests
    {
        [TestCase(" ", Type.EOF, "")]
        [TestCase("\n", Type.EOF, "")]
        [TestCase("\r", Type.EOF, "")]
        public void Lex_IgnoresSpecial(string input, Type type, string value)
        {
            var expected = new List<Token> { new Token(type, value) };
            AssertTokenList(Lex(input), expected);
        }

        [TestCase("1", Type.NUMBER, "1")]
        [TestCase("3.14", Type.NUMBER, "3.14")]
        [TestCase("1_000_000", Type.NUMBER, "1_000_000")]
        public void Lex_Number(string input, Type type, string value)
        {
            var expected = new List<Token> { new Token(type, value) };
            AssertTokenList(Lex(input), expected);
        }

        [TestCase("''", Type.STRING, "")]
        [TestCase("'hello world'", Type.STRING, "hello world")]
        [TestCase("'hello\nworld'", Type.STRING, "hello\nworld")]
        public void Lex_String(string input, Type type, string value)
        {
            var expected = new List<Token> { new Token(type, value)};
            AssertTokenList(Lex(input), expected);
        }

        [TestCase("TEST", Type.IDENTIFIER, "TEST")]
        [TestCase("_TEST", Type.IDENTIFIER, "_TEST")]
        [TestCase("_TEST1", Type.IDENTIFIER, "_TEST1")]
        public void Lex_Identifier(string input, Type type, string value)
        {
            var expected = new List<Token> { new Token(type, value)};
            AssertTokenList(Lex(input), expected);
        }

        [TestCase("A = 1", Type.IDENTIFIER, "A", Type.ASSIGN, "=", Type.NUMBER, "1")]
        public void Lex_Assign(string input, params object[] tokenArgs)
        {
            var expected = new List<Token>();
            for (int i = 0; i < tokenArgs.Length; i += 2)
            {
                expected.Add(new Token((Type)tokenArgs[i], (string)tokenArgs[i + 1]));
            }
            AssertTokenList(Lex(input), expected);
        }

        [TestCase("1 == 1", Type.NUMBER, "1", Type.EQUAL, "==", Type.NUMBER, "1")]
        [TestCase("1 > 1", Type.NUMBER, "1", Type.GT, ">", Type.NUMBER, "1")]
        [TestCase("1 >= 1", Type.NUMBER, "1", Type.GTE, ">=", Type.NUMBER, "1")]
        [TestCase("1 < 1", Type.NUMBER, "1", Type.LT, "<", Type.NUMBER, "1")]
        [TestCase("1 <= 1", Type.NUMBER, "1", Type.LTE, "<=", Type.NUMBER, "1")]
        [TestCase("1 != 1", Type.NUMBER, "1", Type.NOT_EQUAL, "!=", Type.NUMBER, "1")]
        [TestCase("!ABC", Type.NOT, "!", Type.IDENTIFIER, "ABC")]
        public void Lex_Operators(string input, params object[] tokenArgs)
        {
            var expected = new List<Token>();
            for (int i = 0; i < tokenArgs.Length; i += 2)
            {
                expected.Add(new Token((Type)tokenArgs[i], (string)tokenArgs[i + 1]));
            }
            AssertTokenList(Lex(input), expected);
        }

        [Test]
        public void Lex_FunctionCall()
        {
            string input = @"PRINT('1', ' + ', 2)";
            var expected = new List<Token>
            {
                new Token(Type.IDENTIFIER, "PRINT"),
                new Token(Type.PARENTHESES_LEFT, "("),
                new Token(Type.STRING, "1"),
                new Token(Type.COMMA, ","),
                new Token(Type.STRING, " + "),
                new Token(Type.COMMA, ","),
                new Token(Type.NUMBER, "2"),
                new Token(Type.PARENTHESES_RIGHT, ")")
            };
            AssertTokenList(Lex(input), expected);
        }

        private void AssertTokenList(List<Token> actual, List<Token> expected)
        {
            string actualString = string.Join(" ", actual);
            string expectedString = string.Join(" ", expected);
            Assert.That(actualString,Is.EqualTo(expectedString).NoClip); 
        }

        private List<Token> Lex(string input)
        {
            Spelllang.Lexer.Lexer lexer = new Spelllang.Lexer.Lexer(input);
            List<Token> tokens = new List<Token>();

            TokenEnumerator enumerator = lexer.GetEnumerator();
            do
            {
                tokens.Add(enumerator.Current());
                enumerator.Next();
            } while (enumerator.Current().Type != Type.EOF);
            return tokens;
        }
    }
}