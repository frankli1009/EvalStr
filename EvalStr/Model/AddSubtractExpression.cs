using EvalStr.Extensions;
using EvalStr.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvalStr.Model
{
    public class AddSubtractExpression<T> : ArithmeticExpression<T> where T : ICalculator
    {
        public override T GetValue()
        {
            if (this.Operators.Count > 0 && this.Operands.Count > this.Operators.Count)
            {
                T value = this.Operands[0].Value;
                for (int i = 0; i < this.Operators.Count; i++)
                {
                    T op = this.Operands[i + 1].Value;
                    if (this.Operators[i].Operator == OperatorType.Add)
                    {
                        value = (T)value.Add(op);
                    }
                    else
                    {
                        value = (T)value.Subtract(op);
                    }
                }
                return value;
            }
            else
                return base.GetValue();
        }

        public override int Priority()
        {
            return OperatorType.Add.Priority();
        }
    }
}
