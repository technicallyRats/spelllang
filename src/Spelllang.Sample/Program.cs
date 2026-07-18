using System;
using System.Collections.Generic;
using System.IO;
using Spelllang.Builtins;
using Spelllang.Interpreter;
using Spelllang.Interpreter.Importer;
using Spelllang.Lexer;
using Spelllang.Parser;
using Spelllang.Sample;
using Type = Spelllang.Lexer.Type;

public class Program
{
    private static void Main(string[] args)
    {
        var filename = "./sample.spell";
        if (args.Length > 0) filename = args[0];

        Console.WriteLine("Reading file: " + filename);
        var file = new StreamReader(filename);
        var l = new Lexer(file.ReadToEnd());

        var enumerator = l.GetEnumerator();
        Console.WriteLine("-- Tokens --");
        do
        {
            Console.WriteLine(enumerator.Current().Type + enumerator.Current().Value);
            enumerator.Next();
        } while (enumerator.Current().Type != Type.EOF);

        // reset and parse
        var p = new Parser(l);
        var root = p.GetRootProgram();

        if (p.IsFaulty())
        {
            p.DisplayErrors(null);
            throw new Exception("Parsing failed, see above for details");
        }

        var builtins = new List<(string name, IRuntimeBuiltin value)>();
        builtins.Add(("PRINT", new PrintBuiltin()));

        var providedLibrary = new Dictionary<string, string>
        {
            {
                "logging", @"function greet(name) {
PRINT('hello ' + name);
}"
            }
        };
        var resolvers = new List<IImportResolver> { new InMemoryImportResolver(providedLibrary) };

        Console.WriteLine("-- AST --");
        Console.WriteLine(root.ToReadableString());

        var interpreter = new Interpreter(p, builtins, resolvers);

        Console.WriteLine("-- RUNTIME LOGS --");
        var result = interpreter.Run();

        Console.WriteLine("-- FINAL RETURN VALUE --");
        Console.WriteLine(result.ToReadableString());
    }
}