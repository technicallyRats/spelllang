using System;
using System.Collections.Generic;
using Spelllang.Lexer;
using Type = Spelllang.Lexer.Type;

public class Program
{
    private static void Main(string[] args)
    {
        var l = new Lexer(@"'ABC';
    'DEF';
    '';
    PRINT('1', ' + ', 2);
    B='DEF';
    true;
    false;
    A=B;
    A==B;
    3.41;
    1_000;
    1==1;
    PRINT();
    ");

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
        builtins.Add(("PRINT", new Spelllang.Builtins.PrintBuiltin()));

        Console.WriteLine("-- AST --");
        Console.WriteLine(root.ToReadableString());

        var interpreter = new Spelllang.Interpreter.Interpreter(p, builtins);

        Console.WriteLine("-- RUNTIME LOGS --");
        var result = interpreter.Run();

        Console.WriteLine("-- FINAL RETURN VALUE --");
        Console.WriteLine(result.ToReadableString());
    }
}