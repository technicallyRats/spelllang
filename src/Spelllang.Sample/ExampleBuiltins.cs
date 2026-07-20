using System;
using System.Collections.Generic;
using System.Text;
using Spelllang.Builtins;
using Spelllang.Interpreter;

namespace Spelllang.Sample
{
    public struct PrintBuiltin : IRuntimeBuiltin
    {
        public bool CheckParamtypes()
        {
            return false;
        }

        public Type[] GetExpectedParameterTypes()
        {
            return Array.Empty<Type>();
        }

        public IRuntimeVariableBase Call(List<IRuntimeVariableBase?> parameters)
        {
            var sb = new StringBuilder();
            foreach (var param in parameters) sb.Append(param.ToReadableString());
            Console.WriteLine(sb.ToString());
            return new RuntimeNull();
        }

        public string ToReadableString()
        {
            return "Builtin print function";
        }
    }
}