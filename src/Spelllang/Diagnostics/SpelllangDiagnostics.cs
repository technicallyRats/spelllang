using System;

namespace Spelllang.Diagnostics
{
    public static class SpelllangDiagnostics
    {
        private static ISpelllangLogger _logger = new ConsoleLogger();

        public static ISpelllangLogger Logger
        {
            get => _logger;
            set => _logger = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static void Error(string message)
        {
            _logger.Error(message);
        }

        public static void Warn(string message)
        {
            _logger.Warn(message);
        }

        public static void Info(string message)
        {
            _logger.Info(message);
        }

        public static void Reset()
        {
            _logger = new ConsoleLogger();
        }
    }
}