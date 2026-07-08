using System;

namespace Spelllang.Diagnostics
{
    public class ConsoleLogger : ISpelllangLogger
    {
        public void Error(string message)
        {
            Console.WriteLine(message);
        }

        public void Warn(string message)
        {
            Console.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }
    }
}