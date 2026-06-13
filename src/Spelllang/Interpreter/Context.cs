using System.Collections.Generic;

namespace Spelllang.Interpreter
{
    public class Context
    {
        private Dictionary<string, IRuntimeVariableBase> Values = new Dictionary<string, IRuntimeVariableBase>();

        private Context Enclosing;

        public Context(Context enclosing)
        {
            Enclosing = enclosing;
        }

        public Context(Context enclosing, List<(string name, Builtins.IRuntimeBuiltin value)> builtins)
        {
            Enclosing = enclosing;
            foreach ((string name, Builtins.IRuntimeBuiltin value) entry in builtins)
            {
                Register(entry.name, entry.value);
            }
        }

        public void Register(string variableName, IRuntimeVariableBase value)
        {
            Values[variableName] = value;
        }

        public IRuntimeVariableBase Retrieve(string variableName)
        {
            if (Values.ContainsKey(variableName))
            {
                // i dont even know what the end of this means? TODO: learn
                return Values.TryGetValue(variableName, out IRuntimeVariableBase value) ? value : null;
            }
            if (Enclosing == null) { return null; }
            return Enclosing.Retrieve(variableName);
        }

        public void Evict(string variableName)
        {
            Values.Remove(variableName);
        }

        public Context Enclose()
        {
            return new Context(this);
        }
    }
}
