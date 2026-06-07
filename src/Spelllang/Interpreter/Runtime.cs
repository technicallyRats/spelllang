using System.Collections.Generic;
using Spelllang.AST;

namespace Spelllang.Interpreter
{
    public interface IRuntimeVariableBase
    {
        string ToReadableString();
    }

    public interface IRuntimeValueBase<T> : IRuntimeVariableBase
    {
        T GetValue();
    }

    public struct RuntimeString : IRuntimeValueBase<string>
    {
        private string Value { get; }

        public RuntimeString(string value)
        {
            Value = value;
        }

        public string GetValue()
        {
            return Value;
        }

        public string ToReadableString()
        {
            return $"String {Value}";
        }
    }

    public struct RuntimeInt : IRuntimeValueBase<int>
    {
        private int Value { get; }

        public RuntimeInt(int value)
        {
            Value = value;
        }

        public int GetValue()
        {
            return Value;
        }

        public string ToReadableString()
        {
            return $"Int {Value}";
        }
    }

    public struct RuntimeFloat : IRuntimeValueBase<float>
    {
        private readonly float Value;

        public RuntimeFloat(float value)
        {
            Value = value;
        }

        public float GetValue()
        {
            return Value;
        }

        public string ToReadableString()
        {
            return $"Float {Value}";
        }
    }

    public struct RuntimeBoolean : IRuntimeValueBase<bool>
    {
        private bool Value { get; }

        public RuntimeBoolean(bool value)
        {
            Value = value;
        }

        public bool GetValue()
        {
            return Value;
        }

        public string ToReadableString()
        {
            return $"Boolean {Value}";
        }
    }

    public struct RuntimeFunction : IRuntimeValueBase<ProgramNode>
    {
        private ProgramNode Program;
        private readonly List<string> ArgumentNames;

        public RuntimeFunction(ProgramNode program, List<string> argumentNames)
        {
            Program = program;
            ArgumentNames = argumentNames;
        }

        public ProgramNode GetValue()
        {
            return Program;
        }

        public List<string> GetArgumentNames()
        {
            return ArgumentNames;
        }

        public string ToReadableString()
        {
            return $"Function: '{Program.ToReadableString()}' with arguments {ArgumentNames}";
        }
    }

    public struct RuntimeIdentifier : IRuntimeVariableBase
    {
        private readonly string VariableName;

        public RuntimeIdentifier(string variableName)
        {
            VariableName = variableName;
        }

        public string GetVariableName()
        {
            return VariableName;
        }

        public string ToReadableString()
        {
            return $"Identifier '{VariableName}'";
        }
    }

    public struct RuntimeNull : IRuntimeVariableBase
    {
        public string ToReadableString()
        {
            return "NULL";
        }
    }
}