using System;
using System.Collections.Generic;
using System.Linq;
using Spelllang.AST;

namespace Spelllang.Interpreter.Importer
{
    public class ImportUtils
    {
        public static List<IImportResolver> Resolver = new()
        {
            new FilestoreImportResolver()
        };

        private static string ResolveAndReadImport(string importPath, List<IImportResolver> priorityResolver)
        {
            foreach (var resolver in priorityResolver.Concat(Resolver))
                if (resolver.Contains(importPath))
                    return resolver.Read(importPath);

            throw new ArgumentException("Unknown import");
        }

        public static ProgramNode ReadAndParseInput(string importPath)
        {
            return ReadAndParseInput(importPath, new List<IImportResolver>());
        }

        public static ProgramNode ReadAndParseInput(string importPath, List<IImportResolver> priorityResolver)
        {
            // TODO: add resolution to this -> maybe different syntaxes
            // Possibly add extendable sources based on import prefix?
            var l = new Lexer.Lexer(ResolveAndReadImport(importPath, priorityResolver));
            var p = new Parser.Parser(l);
            return p.GetRootProgram();
        }
    }
}