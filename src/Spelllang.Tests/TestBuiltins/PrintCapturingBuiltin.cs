using System;
using System.Collections.Generic;
using Spelllang.Builtins;
using Spelllang.Interpreter;

namespace Spelllang.Tests.TestBuiltins
{
    public class PrintCapturingBuiltin : IRuntimeBuiltin
    {
        public int CallCount { get; private set; }
        public List<IRuntimeVariableBase> LastArgs { get; private set; }

        public PrintCapturingBuiltin()
        {
            LastArgs = new List<IRuntimeVariableBase>();
        }

        public bool CheckParamtypes()
        {
            return false;
        }

        public Type[] GetExpectedParameterTypes()
        {
            return Array.Empty<Type>();
        }

        public IRuntimeVariableBase Call(List<IRuntimeVariableBase> parameters)
        {
            CallCount++;
            LastArgs = parameters;
            return new RuntimeNull();
        }

        public string ToReadableString()
        {
            return "PrintCapturingBuiltin";
        }

        public override string ToString()
        {
            return ToReadableString();
        }
    }
}
