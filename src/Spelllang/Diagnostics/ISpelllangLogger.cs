namespace Spelllang.Diagnostics
{
    public interface ISpelllangLogger
    {
        void Error(string message);

        void Warn(string message);

        void Info(string message);
    }
}