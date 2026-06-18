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
                case "!=":
                    return Invert(RunEqual(left, right));
                case "+":
                    return RunPlus(left, right);
                case "-":
                    return RunMinus(left, right);
                case "*":
                    return RunMultiply(left, right);
                case "/":
                    return RunDivide(left, right);
                case "%":
                    return RunModulo(left, right);
                case ">":
                    return RunGreaterThan(left, right);
                case ">=":
                    return RunOr(RunGreaterThan(left, right), RunEqual(left, right));
                case "<":
                    return RunLessThan(left, right);
                case "<=":
                    return RunOr(RunLessThan(left, right), RunEqual(left, right));
                case "||":
                    return RunOr(left, right);
                case "&&":
                    return RunAnd(left, right);
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

        private static IRuntimeVariableBase RunPlus(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                Console.WriteLine("For now only addition between two variables of the same type is supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<int> leftInt && right is IRuntimeValueBase<int> rightInt)
                return new RuntimeInt(leftInt.GetValue() + rightInt.GetValue());

            if (left is IRuntimeValueBase<float> leftFloat && right is IRuntimeValueBase<float> rightFloat)
                return new RuntimeFloat(leftFloat.GetValue() + rightFloat.GetValue());

            return new RuntimeNull();
        }

        private static IRuntimeVariableBase RunMinus(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                Console.WriteLine("For now only comparison between two variables of the same type are supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<int> leftInt && right is IRuntimeValueBase<int> rightInt)
                return new RuntimeInt(leftInt.GetValue() - rightInt.GetValue());

            if (left is IRuntimeValueBase<float> leftFloat && right is IRuntimeValueBase<float> rightFloat)
                return new RuntimeFloat(leftFloat.GetValue() - rightFloat.GetValue());

            return new RuntimeNull();
        }

        private static IRuntimeVariableBase RunMultiply(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                Console.WriteLine("For now only multiplication between two variables of the same type is supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<int> leftInt && right is IRuntimeValueBase<int> rightInt)
                return new RuntimeInt(leftInt.GetValue() * rightInt.GetValue());

            if (left is IRuntimeValueBase<float> leftFloat && right is IRuntimeValueBase<float> rightFloat)
                return new RuntimeFloat(leftFloat.GetValue() * rightFloat.GetValue());

            return new RuntimeNull();
        }

        private static IRuntimeVariableBase RunDivide(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                Console.WriteLine("For now only division between two variables of the same type is supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<int> leftInt && right is IRuntimeValueBase<int> rightInt)
                return new RuntimeInt(leftInt.GetValue() / rightInt.GetValue());

            if (left is IRuntimeValueBase<float> leftFloat && right is IRuntimeValueBase<float> rightFloat)
                return new RuntimeFloat(leftFloat.GetValue() / rightFloat.GetValue());

            return new RuntimeNull();
        }

        private static IRuntimeVariableBase RunModulo(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                Console.WriteLine("For now only modulo between two variables of the same type is supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<int> leftInt && right is IRuntimeValueBase<int> rightInt)
                return new RuntimeInt(leftInt.GetValue() % rightInt.GetValue());

            if (left is IRuntimeValueBase<float> leftFloat && right is IRuntimeValueBase<float> rightFloat)
                return new RuntimeFloat(leftFloat.GetValue() % rightFloat.GetValue());

            return new RuntimeNull();
        }

        private static IRuntimeVariableBase RunGreaterThan(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                Console.WriteLine("For now only comparison between two variables of the same type are supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<int> leftInt && right is IRuntimeValueBase<int> rightInt)
                return new RuntimeBoolean(leftInt.GetValue() > rightInt.GetValue());

            if (left is IRuntimeValueBase<float> leftFloat && right is IRuntimeValueBase<float> rightFloat)
                return new RuntimeBoolean(leftFloat.GetValue() > rightFloat.GetValue());

            return new RuntimeNull();
        }

        private static IRuntimeVariableBase RunLessThan(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                Console.WriteLine("For now only comparison between two variables of the same type are supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<int> leftInt && right is IRuntimeValueBase<int> rightInt)
                return new RuntimeBoolean(leftInt.GetValue() < rightInt.GetValue());

            if (left is IRuntimeValueBase<float> leftFloat && right is IRuntimeValueBase<float> rightFloat)
                return new RuntimeBoolean(leftFloat.GetValue() < rightFloat.GetValue());

            return new RuntimeNull();
        }

        private static IRuntimeVariableBase RunAnd(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                Console.WriteLine("For now only comparison between two variables of the same type are supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<bool> leftBool && right is IRuntimeValueBase<bool> rightBool)
                return new RuntimeBoolean(leftBool.GetValue() && rightBool.GetValue());

            return new RuntimeBoolean(false);
        }

        private static IRuntimeVariableBase RunOr(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                Console.WriteLine("For now only comparison between two variables of the same type are supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<bool> leftBool && right is IRuntimeValueBase<bool> rightBool)
                return new RuntimeBoolean(leftBool.GetValue() || rightBool.GetValue());

            return new RuntimeBoolean(false);
        }

        private static IRuntimeVariableBase Invert(IRuntimeVariableBase value)
        {
            if (value is RuntimeBoolean runtimeBoolean) return new RuntimeBoolean(!runtimeBoolean.GetValue());
            return new RuntimeBoolean(false);
        }
    }
}