using System.Collections.Generic;

namespace Spelllang.Interpreter.Importer
{
    public class InMemoryImportResolver : IImportResolver
    {
        private readonly Dictionary<string, string> _content = new();

        public InMemoryImportResolver(Dictionary<string, string> content)
        {
            _content = content;
        }

        public string Name()
        {
            return "InMemoryResolver";
        }

        public bool Contains(string importPath)
        {
            return _content.ContainsKey(importPath);
        }

        public string Read(string importPath)
        {
            return _content[importPath];
        }
    }
}