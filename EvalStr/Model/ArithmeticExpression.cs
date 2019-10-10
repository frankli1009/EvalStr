using EvalStr.Extensions;
using EvalStr.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvalStr.Model
{
    public class ArithmeticExpression<T> : IArithmeticExpression<T> where T : ICalculator
    {
        public List<ArithmeticOperator> Operators { get; }
        public List<ArithmeticOperand<T>> Operands { get; }
        public ArithmeticExpression<T> ParentExpression { get; set; }
        protected int OperandCount { get; set; }

        public ArithmeticExpression()
        {
            Operators = new List<ArithmeticOperator>();
            Operands = new List<ArithmeticOperand<T>>();
            OperandCount = 2;
        }

        public virtual T GetValue() 
        {
            if (!Operators.Any() && Operands.Any())
            {
                return Operands[0].Value;
            }
            else
            {
                return default(T);
            }
        }

        public virtual int Priority()
        {
            return OperatorType.None.Priority();
        }

        public virtual void SetOperandsParentExpression(ArithmeticExpression<T> parentExpression)
        {
            foreach (var item in Operands)
            {
                if (item.IsExpression)
                {
                    item.Expression.ParentExpression = parentExpression;
                }
            }
        }

        public void AddOperand(ArithmeticOperand<T> newOperand)
        {
            if (newOperand != null)
            {
                if (newOperand.IsExpression)
                {
                    newOperand.Expression.ParentExpression = this;
                }
                this.Operands.Add(newOperand);
            }
        }

        public void AddOperand(ArithmeticExpression<T> newOperandExpression)
        {
            if (newOperandExpression != null)
            {
                var newOperand = new ArithmeticOperand<T>
                {
                    Expression = newOperandExpression
                };
                AddOperand(newOperand);
            }
        }

        public void AddOperands(IEnumerable<ArithmeticOperand<T>> operands)
        {
            if (operands != null)
            {
                foreach (var item in operands)
                {
                    AddOperand(item);
                }
            }
        }
    }
}
