using System.Collections.Generic;
using Spelllang.Builtins;

namespace Spelllang.Interpreter
{
    public class Context
    {
        private readonly IRuntimeBuiltin? _builtin;
        private readonly Context? _enclosing;
        private readonly IRuntimeVariableBase? _value;
        private readonly Dictionary<string, Context> _values = new();

        public Context(Context enclosing)
        {
            _enclosing = enclosing;
        }

        public Context(Context? enclosing, IRuntimeVariableBase value)
        {
            _enclosing = enclosing;
            _value = value;
        }

        public Context(Context? enclosing, IRuntimeBuiltin builtin)
        {
            _builtin = builtin;
        }

        public Context(Context? enclosing, List<(string name, IRuntimeBuiltin value)> builtins)
        {
            _enclosing = enclosing;
            foreach (var entry in builtins) Register(entry.name, new Context(null, entry.value));
        }

        public void Register(string variableName, Context value)
        {
            _values[variableName] = value;
        }

        public void Register(string variableName, IRuntimeVariableBase value)
        {
            _values[variableName] = new Context(null, value);
        }

        public IRuntimeVariableBase Retrieve(string fullVariableName)
        {
            if (fullVariableName.Length == 0)
            {
                if (_value != null) return _value;
                if (_builtin != null) return _builtin;
                return new RuntimeError(
                    "Variable resolution 'died' in a context with no value");
            }

            var variableComponentRelevantForCurrentLevel = GetRelevantVariableNameComponent(fullVariableName);

            if (!_values.ContainsKey(variableComponentRelevantForCurrentLevel))
                if (_enclosing != null)
                    return _enclosing.Retrieve(fullVariableName);

            var variableComponentRelevantForNextLevel = "";
            if (fullVariableName.Contains("."))
                variableComponentRelevantForNextLevel = fullVariableName.Substring(fullVariableName.IndexOf('.') + 1);

            if (!_values.ContainsKey(variableComponentRelevantForCurrentLevel))
                return new RuntimeError(
                    $"Tried retrieving unknown variable {fullVariableName} ({variableComponentRelevantForCurrentLevel})");

            return _values[variableComponentRelevantForCurrentLevel].Retrieve(variableComponentRelevantForNextLevel);
        }

        private string GetRelevantVariableNameComponent(string initialValue)
        {
            if (initialValue.Contains("."))
                return initialValue.Substring(0,
                    initialValue.IndexOf('.')
                );
            return initialValue;
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
            _values[prefix] = context;
        }

        private bool IsIntermediate()
        {
            return _value == null;
        }
    }
}