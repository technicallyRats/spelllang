using System;
using System.Collections.Generic;
using System.Text;
using Spelllang.Interpreter;

namespace Spelllang.Builtins
{
    public struct PrintBuiltin : IRuntimeBuiltin
    {
        public bool CheckParamtypes() => false;

        public Type[] GetExpectedParameterTypes() => Array.Empty<Type>();

        public IRuntimeVariableBase Call(List<IRuntimeVariableBase> parameters)
        {
            StringBuilder sb = new StringBuilder();
            foreach (IRuntimeVariableBase param in parameters)
            {
                sb.Append(param.ToReadableString());
            }
            Console.WriteLine(sb.ToString());
            return new RuntimeNull();
        }

        public string ToReadableString() => "Builtin print function";
    }
}
