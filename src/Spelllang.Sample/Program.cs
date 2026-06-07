using System;
using System.Collections.Generic;
using Spelllang.Lexer;
using Type = Spelllang.Lexer.Type;

public class Program
{
    
    static void Main(string[] args)
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

        // reset and parse
        Spelllang.Parser.Parser p = new Spelllang.Parser.Parser(new TokenEnumerator(input));
        var root = p.GetRootProgram();

        var builtins = new List<(string name, Spelllang.Builtins.IRuntimeBuiltin value)>();
        builtins.Add(("PRINT", new Spelllang.Builtins.PrintBuiltin()));

        Console.WriteLine("-- AST --");
        Console.WriteLine(root.ToReadableString());

        Spelllang.Interpreter.Interpreter interpreter = new Spelllang.Interpreter.Interpreter(p, builtins);

        Console.WriteLine("-- RUNTIME LOGS --");
        var result = interpreter.Run();

        Console.WriteLine("-- FINAL RETURN VALUE --");
        Console.WriteLine(result.ToReadableString());
    }
    
    static void Main2(string[] args)
    {
        Spelllang.Lexer.Lexer l = new Spelllang.Lexer.Lexer(@"'ABC';
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
    ");

        var enumerator = l.GetEnumerator();
        Console.WriteLine("-- Tokens --");
        do
        {
            Console.WriteLine(enumerator.Current().Type + enumerator.Current().Value);
            enumerator.Next();
        } while (enumerator.Current().Type != Spelllang.Lexer.Type.EOF);

        // reset and parse
        Spelllang.Parser.Parser p = new Spelllang.Parser.Parser(l);
        var root = p.GetRootProgram();

        var builtins = new List<(string name, Spelllang.Builtins.IRuntimeBuiltin value)>();
        builtins.Add(("PRINT", new Spelllang.Builtins.PrintBuiltin()));

        Console.WriteLine("-- AST --");
        Console.WriteLine(root.ToReadableString());

        Spelllang.Interpreter.Interpreter interpreter = new Spelllang.Interpreter.Interpreter(p, builtins);

        Console.WriteLine("-- RUNTIME LOGS --");
        var result = interpreter.Run();

        Console.WriteLine("-- FINAL RETURN VALUE --");
        Console.WriteLine(result.ToReadableString());
    }
}