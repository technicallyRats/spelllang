using System;
using System.Collections.Generic;
using Spelllang.Builtins;
using Type = Spelllang.Lexer.Type;

namespace Spelllang
{
    public class Program
    {

        static void Main(string[] args)
        {
            Lexer.Lexer l = new Lexer.Lexer(@"'ABC';
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
            } while (enumerator.Current().Type != Type.EOF);

            // reset and parse
            Parser.Parser p = new Parser.Parser(l);
            var root = p.GetRootProgram();

            var builtins = new List<(string name, IRuntimeBuiltin value)>();
            builtins.Add(("PRINT", new PrintBuiltin()));

            Console.WriteLine("-- AST --");
            Console.WriteLine(root.ToReadableString());

            Interpreter.Interpreter interpreter = new Interpreter.Interpreter(p, builtins);

            Console.WriteLine("-- RUNTIME LOGS --");
            var result = interpreter.Run();

            Console.WriteLine("-- FINAL RETURN VALUE --");
            Console.WriteLine(result.ToReadableString());
        }
    }
}