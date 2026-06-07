using System.Text;
using Spelllang.Lexer;
using NUnit.Framework;

namespace Spelllang.Tests.Lexer
{
    [TestFixture]
    public class LexerTests
    {
        [TestCase(" ", "EOF->\n")]
        [TestCase("\n", "EOF->\n")]
        [TestCase("\r", "EOF->\n")]
        public void Lex_IgnoresSpecial(string input, string expected)
        {
            Assert.AreEqual(expected, Lex(input));
        }
        
        [TestCase("1", "NUMBER->1\n")]
        [TestCase("3.14", "NUMBER->3.14\n")]
        [TestCase("1_000_000", "NUMBER->1_000_000\n")]
        public void Lex_Number(string input, string expected)
        {
            Assert.AreEqual(expected, Lex(input));
        }
        
        [TestCase("''", "STRING->\n")]
        [TestCase("'hello world'", "STRING->hello world\n")]
        [TestCase("'hello\nworld'", "STRING->hello\nworld\n")]
        public void Lex_String(string input, string expected)
        {
            Assert.AreEqual(expected, Lex(input));
        }
        
        [TestCase("TEST", "IDENTIFIER->TEST\n")]
        [TestCase("_TEST", "IDENTIFIER->_TEST\n")]
        [TestCase("_TEST1", "IDENTIFIER->_TEST1\n")]
        public void Lex_Identifier(string input, string expected)
        {
            Assert.AreEqual(expected, Lex(input));
        }

        private string Lex(string input)
        {
            Spelllang.Lexer.Lexer lexer = new Spelllang.Lexer.Lexer(input); 
            StringBuilder sb = new StringBuilder();
                
            TokenEnumerator enumerator = lexer.GetEnumerator();
            do
            {
                sb.AppendLine(enumerator.Current().Type + "->"+ enumerator.Current().Value);
                enumerator.Next();
            } while (enumerator.Current().Type != Type.EOF);

            return sb.ToString();
        }
    }

}