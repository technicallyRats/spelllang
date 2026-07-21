using System.Collections.Generic;
using Spelllang.Diagnostics;

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
                    SpelllangDiagnostics.Error("Unsupported operator " + op);
                    return new RuntimeNull();
            }
        }

        public static IRuntimeVariableBase RunPrefixExpression(string op, IRuntimeVariableBase expr)
        {
            switch (op)
            {
                case "!":
                    // assumes that this surely will be int
                    return Invert(expr);
                case "+":
                    return RunPlus(expr);
                case "-":
                    return RunMinus(expr);
                default:
                    SpelllangDiagnostics.Error("Unsupported operator " + op);
                    return new RuntimeNull();
            }
        }

        private static IRuntimeVariableBase RunEqual(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                SpelllangDiagnostics.Error(
                    "For now only comparison between two variables of the same type are supported");
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

            SpelllangDiagnostics.Error("For now equal between two variables of this type is not supported");

            return new RuntimeNull();
        }

        private static IRuntimeVariableBase RunPlus(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                SpelllangDiagnostics.Error("For now only addition between two variables of the same type is supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<int> leftInt && right is IRuntimeValueBase<int> rightInt)
                return new RuntimeInt(leftInt.GetValue() + rightInt.GetValue());

            if (left is IRuntimeValueBase<float> leftFloat && right is IRuntimeValueBase<float> rightFloat)
                return new RuntimeFloat(leftFloat.GetValue() + rightFloat.GetValue());

            if (left is IRuntimeValueBase<string> leftString && right is IRuntimeValueBase<string> rightString)
                return new RuntimeString(leftString.GetValue() + rightString.GetValue());

            if (left is RuntimeList leftList && right is RuntimeList rightList)
            {
                // TODO: This is slow
                var mergedList = new List<IRuntimeVariableBase>(leftList.GetContent());
                mergedList.AddRange(rightList.GetContent());
                return new RuntimeList(mergedList);
            }

            SpelllangDiagnostics.Error("For now addition between two variables of this type is not supported");

            return new RuntimeNull();
        }

        private static IRuntimeVariableBase RunMinus(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                SpelllangDiagnostics.Error(
                    "For now only comparison between two variables of the same type are supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<int> leftInt && right is IRuntimeValueBase<int> rightInt)
                return new RuntimeInt(leftInt.GetValue() - rightInt.GetValue());

            if (left is IRuntimeValueBase<float> leftFloat && right is IRuntimeValueBase<float> rightFloat)
                return new RuntimeFloat(leftFloat.GetValue() - rightFloat.GetValue());

            SpelllangDiagnostics.Error("For now comparison between two variables of this type is not supported");

            return new RuntimeNull();
        }

        private static IRuntimeVariableBase RunMultiply(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                SpelllangDiagnostics.Error(
                    "For now only multiplication between two variables of the same type is supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<int> leftInt && right is IRuntimeValueBase<int> rightInt)
                return new RuntimeInt(leftInt.GetValue() * rightInt.GetValue());

            if (left is IRuntimeValueBase<float> leftFloat && right is IRuntimeValueBase<float> rightFloat)
                return new RuntimeFloat(leftFloat.GetValue() * rightFloat.GetValue());

            SpelllangDiagnostics.Error("For now multiplication between two variables of this type is not supported");

            return new RuntimeNull();
        }

        private static IRuntimeVariableBase RunDivide(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                SpelllangDiagnostics.Error("For now only division between two variables of the same type is supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<int> leftInt && right is IRuntimeValueBase<int> rightInt)
                return new RuntimeInt(leftInt.GetValue() / rightInt.GetValue());

            if (left is IRuntimeValueBase<float> leftFloat && right is IRuntimeValueBase<float> rightFloat)
                return new RuntimeFloat(leftFloat.GetValue() / rightFloat.GetValue());

            SpelllangDiagnostics.Error("For now division between two variables of this type is not supported");

            return new RuntimeNull();
        }

        private static IRuntimeVariableBase RunModulo(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                SpelllangDiagnostics.Error("For now only modulo between two variables of the same type is supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<int> leftInt && right is IRuntimeValueBase<int> rightInt)
                return new RuntimeInt(leftInt.GetValue() % rightInt.GetValue());

            if (left is IRuntimeValueBase<float> leftFloat && right is IRuntimeValueBase<float> rightFloat)
                return new RuntimeFloat(leftFloat.GetValue() % rightFloat.GetValue());

            SpelllangDiagnostics.Error("For now modulo between two variables of this type is not supported");

            return new RuntimeNull();
        }

        private static IRuntimeVariableBase RunGreaterThan(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                SpelllangDiagnostics.Error(
                    "For now only comparison between two variables of the same type are supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<int> leftInt && right is IRuntimeValueBase<int> rightInt)
                return new RuntimeBoolean(leftInt.GetValue() > rightInt.GetValue());

            if (left is IRuntimeValueBase<float> leftFloat && right is IRuntimeValueBase<float> rightFloat)
                return new RuntimeBoolean(leftFloat.GetValue() > rightFloat.GetValue());

            SpelllangDiagnostics.Error("For now gt between two variables of this type is not supported");

            return new RuntimeNull();
        }

        private static IRuntimeVariableBase RunLessThan(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                SpelllangDiagnostics.Error(
                    "For now only comparison between two variables of the same type are supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<int> leftInt && right is IRuntimeValueBase<int> rightInt)
                return new RuntimeBoolean(leftInt.GetValue() < rightInt.GetValue());

            if (left is IRuntimeValueBase<float> leftFloat && right is IRuntimeValueBase<float> rightFloat)
                return new RuntimeBoolean(leftFloat.GetValue() < rightFloat.GetValue());

            SpelllangDiagnostics.Error("For now lt between two variables of this type is not supported");

            return new RuntimeNull();
        }

        private static IRuntimeVariableBase RunAnd(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                SpelllangDiagnostics.Error(
                    "For now only comparison between two variables of the same type are supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<bool> leftBool && right is IRuntimeValueBase<bool> rightBool)
                return new RuntimeBoolean(leftBool.GetValue() && rightBool.GetValue());

            SpelllangDiagnostics.Error("For now and between two variables of this type is not supported");

            return new RuntimeBoolean(false);
        }

        private static IRuntimeVariableBase RunOr(IRuntimeVariableBase left, IRuntimeVariableBase right)
        {
            if (left.GetType() != right.GetType())
            {
                SpelllangDiagnostics.Error(
                    "For now only comparison between two variables of the same type are supported");
                return new RuntimeNull();
            }

            if (left is IRuntimeValueBase<bool> leftBool && right is IRuntimeValueBase<bool> rightBool)
                return new RuntimeBoolean(leftBool.GetValue() || rightBool.GetValue());

            SpelllangDiagnostics.Error("For now or between two variables of this type is not supported");

            return new RuntimeBoolean(false);
        }

        private static IRuntimeVariableBase RunPlus(IRuntimeVariableBase expr)
        {
            if (expr is IRuntimeValueBase<int> exprInt)
                return new RuntimeInt(+exprInt.GetValue());
            if (expr is IRuntimeValueBase<float> exprFloat)
                return new RuntimeFloat(+exprFloat.GetValue());


            SpelllangDiagnostics.Error("Prefix operation '+' not supported for " + expr.GetType().Name);

            return new RuntimeNull();
        }

        private static IRuntimeVariableBase RunMinus(IRuntimeVariableBase expr)
        {
            if (expr is IRuntimeValueBase<int> exprInt)
                return new RuntimeInt(-exprInt.GetValue());
            if (expr is IRuntimeValueBase<float> exprFloat)
                return new RuntimeFloat(-exprFloat.GetValue());


            SpelllangDiagnostics.Error("Prefix operation '-' not supported for " + expr.GetType().Name);

            return new RuntimeNull();
        }

        // TODO: Other things should probably count as truthy as well...
        // If that is implemented it affects some of the boolean operators, maybe
        public static bool IsTruthy(IRuntimeVariableBase expr)
        {
            if (expr is RuntimeBoolean boolExpr) return boolExpr.GetValue();
            return false;
        }

        private static IRuntimeVariableBase Invert(IRuntimeVariableBase value)
        {
            if (value is RuntimeBoolean runtimeBoolean) return new RuntimeBoolean(!runtimeBoolean.GetValue());
            return new RuntimeBoolean(false);
        }
    }
}