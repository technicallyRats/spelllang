using System.Collections.Generic;
using Spelllang.Builtins;

namespace Spelllang.Interpreter
{
    public class Context
    {
        private readonly Context? _enclosing;
        private readonly Dictionary<string, Context> _imported = new();
        private readonly Dictionary<string, IRuntimeVariableBase> _values = new();

        public Context(Context enclosing)
        {
            _enclosing = enclosing;
        }

        public Context(Context? enclosing, List<(string name, IRuntimeBuiltin value)> builtins)
        {
            _enclosing = enclosing;
            foreach (var entry in builtins) Register(entry.name, entry.value);
        }

        public void Register(string variableName, IRuntimeVariableBase? value)
        {
            _values[variableName] = value;
        }

        public IRuntimeVariableBase? Retrieve(string variableName)
        {
            if (variableName.Contains("."))
            {
                var nestingVariable = variableName.Substring(0, variableName.IndexOf('.'));
                return _imported[nestingVariable].Retrieve(variableName.Substring(variableName.IndexOf('.') + 1));
            }

            if (_values.ContainsKey(variableName))
                // i dont even know what the end of this means? TODO: learn
                return _values.TryGetValue(variableName, out var value) ? value : null;
            if (_enclosing == null) return null;
            return _enclosing.Retrieve(variableName);
        }

        public void Evict(string variableName)
        {
            _values.Remove(variableName);
        }

        public Context Enclose()
        {
            return new Context(this);
        }

        // merge different context into this
        // may move upward until global, will overwrite existing values on key collision
        // prefix will be prepended as <prefix>.<value>
        public void Merge(Context other, string prefix, bool global)
        {
            var target = this;
            while (target._enclosing != null && global) target = target._enclosing;
            if (prefix.Length > 0)
                target.Import(other, prefix);
            else
                foreach (var key in other._values.Keys)
                {
                    var mergeKey = key;
                    target.Register(mergeKey, other._values[key]);
                }
        }

        private void Import(Context context, string prefix)
        {
            _imported[prefix] = context;
        }
    }
}