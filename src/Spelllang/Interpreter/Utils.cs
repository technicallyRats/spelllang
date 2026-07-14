using System.IO;
using Spelllang.AST;

namespace Spelllang.Interpreter
{
    public class Utils
    {
        public static ProgramNode readAndParseInput(string importPath)
        {
            // TODO: add resolution to this -> maybe different syntaxes
            // Possibly add extendable sources based on import prefix?
            var file = new StreamReader(importPath);
            var l = new Lexer.Lexer(file.ReadToEnd());
            var p = new Parser.Parser(l);
            return p.GetRootProgram();
        }
    }
}