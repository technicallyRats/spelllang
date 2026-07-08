using System;
using System.Collections.Generic;
using Spelllang.Lexer;
using Type = Spelllang.Lexer.Type;

public class Program
{
    private static void Main(string[] args)
    {
        var filename = "./sample.spell";
        if (args.Length > 0) filename = args[0];

        Console.WriteLine("Reading file: " + filename);
        var file = new System.IO.StreamReader(filename);
        var l = new Lexer(file.ReadToEnd());

        var enumerator = l.GetEnumerator();
        Console.WriteLine("-- Tokens --");
        do
        {
            Console.WriteLine(enumerator.Current().Type + enumerator.Current().Value);
            enumerator.Next();
        } while (enumerator.Current().Type != Type.EOF);

        // reset and parse
        var p = new Spelllang.Parser.Parser(l);
        var root = p.GetRootProgram();

        var builtins = new List<(string name, Spelllang.Builtins.IRuntimeBuiltin value)>();
        builtins.Add(("PRINT", new Spelllang.Sample.PrintBuiltin()));

        Console.WriteLine("-- AST --");
        Console.WriteLine(root.ToReadableString());

        var interpreter = new Spelllang.Interpreter.Interpreter(p, builtins);

        Console.WriteLine("-- RUNTIME LOGS --");
        var result = interpreter.Run();

        Console.WriteLine("-- FINAL RETURN VALUE --");
        Console.WriteLine(result.ToReadableString());
    }
}