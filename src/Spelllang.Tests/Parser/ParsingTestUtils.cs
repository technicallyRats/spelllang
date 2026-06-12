using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.AST;
using Spelllang.Lexer;

namespace Spelllang.Tests.Parser
{
    public static class ParsingTestUtils
    {
        
        public static void AssertAst(ProgramNode actual, ProgramNode  expected)
        {
            Assert.That(actual.ToReadableString(),Is.EqualTo(expected.ToReadableString()).NoClip); 
        }
        
       public static ProgramNode Parse(List<Token> input)
        {
            TokenEnumerator enumerator = new TokenEnumerator(input);
            Spelllang.Parser.Parser parser = new Spelllang.Parser.Parser(enumerator);
            return parser.GetRootProgram();
        }    }
}