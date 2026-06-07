using System;
using System.Collections.Generic;
using Spelllang.Interpreter;

namespace Spelllang.Builtins
{
    public interface IRuntimeBuiltin : IRuntimeVariableBase
    {
        bool CheckParamtypes();
        Type[] GetExpectedParameterTypes();
        
        IRuntimeVariableBase Call(List<IRuntimeVariableBase> parameters);
    }
    
    public static class BridgeValidator {
        public static bool ParamsMatchExpectedTypes(IRuntimeBuiltin builtin, List<IRuntimeVariableBase> parameters)
        {
            if (!builtin.CheckParamtypes())
            {
                return true;
            }

            Type[] expectedTypes = builtin.GetExpectedParameterTypes();

            // Check if the number of parameters matches the expected types
            if (parameters.Count != expectedTypes.Length)
                return false;

            for (int i = 0; i < parameters.Count; i++)
            {
                Type expectedType = expectedTypes[i];
                Type actualType = parameters[i].GetType();

                // Check if the actual type matches the expected type
                if (!actualType.Equals(expectedType))
                    return false;
            }

            return true;
        }
    }
}
