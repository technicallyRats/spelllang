namespace Spelllang.Interpreter.Importer
{
    public interface IImportResolver
    {
        public bool Contains(string importPath);
        public string Read(string importPath);
    }
}