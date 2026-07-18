namespace Spelllang.Interpreter.Importer
{
    public interface IImportResolver
    {
        public string Name();
        public bool Contains(string importPath);
        public string Read(string importPath);
    }
}