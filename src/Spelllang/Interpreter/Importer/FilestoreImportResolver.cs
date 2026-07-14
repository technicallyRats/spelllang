using System.IO;

namespace Spelllang.Interpreter.Importer
{
    public class FilestoreImportResolver : IImportResolver
    {
        public bool Contains(string importPath)
        {
            return File.Exists(importPath);
        }

        public string Read(string importPath)
        {
            return File.ReadAllText(importPath);
        }
    }
}