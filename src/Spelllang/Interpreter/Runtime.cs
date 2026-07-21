using System.Collections.Generic;
using System.Text;
using Spelllang.AST;

namespace Spelllang.Interpreter
{
    public interface IRuntimeVariableBase
    {
        string ToReadableString();
    }

    public struct RuntimeError : IRuntimeVariableBase
    {
        private string Reason { get; }

        public RuntimeError(string reason)
        {
            Reason = reason;
        }

        public string ToReadableString()
        {
            return $"Error {Reason}";
        }

        public string GetReason()
        {
            return Reason;
        }
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

    public struct RuntimeList : IRuntimeVariableBase
    {
        private readonly List<IRuntimeVariableBase> Content;

        public RuntimeList(List<IRuntimeVariableBase> content)
        {
            Content = content;
        }

        public List<IRuntimeVariableBase> GetContent()
        {
            return Content;
        }

        public string ToReadableString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("List ");
            foreach (var c in Content) stringBuilder.Append(c.ToReadableString());
            return stringBuilder.ToString();
        }

        public IRuntimeVariableBase Index(IRuntimeVariableBase index)
        {
            // TODO: This could be a decorator guard maybe?
            if (index is RuntimeError) return index;

            if (index is RuntimeInt runtimeInt)
            {
                if (runtimeInt.GetValue() > Content.Count - 1 || runtimeInt.GetValue() < 0)
                    return new RuntimeError(
                        $"Index value of {runtimeInt.GetValue()} is not valid for this list. Out of bounds.");
                return Content[runtimeInt.GetValue()];
            }

            return new RuntimeError(
                $"Index type of {index.ToReadableString()} is not supported for type list.");
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