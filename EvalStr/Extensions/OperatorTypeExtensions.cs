using EvalStr.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvalStr.Extensions
{
    public static class OperatorTypeExtensions
    {
        public static int Priority(this OperatorType op)
        {
            switch (op)
            {
                case OperatorType.Add:
                case OperatorType.Subtract:
                    return 100;
                case OperatorType.Multiply:
                case OperatorType.Divide:
                    return 500;
                default:
                    return 0;
            }
        }

        public static bool TakePrecedenceOver(this OperatorType op, OperatorType op1)
        {
            return op.Priority() > op1.Priority();
        }

        public static ArithmeticOperator FromChar(this ArithmeticOperator op, char opchar)
        {
            if (op != null)
            {
                switch (opchar)
                {
                    case '+':
                        op.Operator = OperatorType.Add;
                        break;
                    case '-':
                        op.Operator = OperatorType.Subtract;
                        break;
                    case '*':
                        op.Operator = OperatorType.Multiply;
                        break;
                    case '/':
                        op.Operator = OperatorType.Divide;
                        break;
                    default:
                        op.Operator = OperatorType.None;
                        break;
                }
            }
            return op;
        }
    }
}
