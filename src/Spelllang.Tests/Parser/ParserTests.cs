using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Spelllang.AST;
using Spelllang.Lexer;

[TestFixture]
public class ParserTests
{
        [Test]
        public void Parse_FunctionCall()
        {
            List<Token> input =new List<Token>
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
            ProgramNode expected = new ProgramNode(new List<IStatementNode>
            {
                new ExpressionStatement(
                    new CallExpression(
                        new IdentifierExpression("PRINT"),
                        new List<IExpressionNode>
                        {
                            new StringExpression("1"),
                            new StringExpression(" + "),
                            new IntegerExpression(2)
                        }
                        )
                    )
            });
            AssertAst(Parse(input), expected);
        }
        
        private void AssertAst(ProgramNode actual, ProgramNode  expected)
        {
            Assert.That(actual.ToReadableString(),Is.EqualTo(expected.ToReadableString()).NoClip); 
        }

        private ProgramNode Parse(List<Token> input)
        {
            TokenEnumerator enumerator = new TokenEnumerator(input);
            Spelllang.Parser.Parser parser = new Spelllang.Parser.Parser(enumerator);
            return parser.GetRootProgram();
        }
        }