using System;

namespace Spelllang.Interpreter
{
    public static class Operations
    {
        public static IRuntimeVariableBase RunInfixExpression(string op, IRuntimeVariableBase left,
            IRuntimeVariableBase right)
        {
            switch (op)
            {
                case "==":
                    return RunEqual(left, right);
                default:
                    Console.WriteLine("Unsupported operator " + op);
                    return new RuntimeNull();
            }
        }

        private static IRuntimeVariableBase RunEqual(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                Console.WriteLine("For now only comparison between two variables of the same type are supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<int> leftInt && right is IRuntimeValueBase<int> rightInt)
                return new RuntimeBoolean(leftInt.GetValue() == rightInt.GetValue());

            if (left is IRuntimeValueBase<float> leftFloat && right is IRuntimeValueBase<float> rightFloat)
                return new RuntimeBoolean(leftFloat.GetValue() == rightFloat.GetValue());

            if (left is IRuntimeValueBase<string> leftString && right is IRuntimeValueBase<string> rightString)
                return new RuntimeBoolean(leftString.GetValue() == rightString.GetValue());

            if (left is IRuntimeValueBase<bool> leftBool && right is IRuntimeValueBase<bool> rightBool)
                return new RuntimeBoolean(leftBool.GetValue() == rightBool.GetValue());

            return new RuntimeNull();
        }
    }
}