using EvalStr.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvalStr.Model
{
    public class ArithmeticOperand<T> where T: ICalculator
    {
        private T _value;
        private ArithmeticExpression<T> _expression;
        private bool _isExpression;
        public T SimpleValue
        {
            get
            {
                return _isExpression ? default(T) : _value;
            }
            set
            {
                _value = value;
                _isExpression = false;
            }
        }

        public ArithmeticExpression<T> Expression
        {
            get
            {
                return _isExpression ? _expression : null;
            }
            set
            {
                _isExpression = true;
                _expression = value;
            }
        }

        public T Value
        {
            get
            {
                return _isExpression ? _expression.GetValue() : _value;
            }
        }

        public bool IsExpression
        {
            get
            {
                return _isExpression;
            }
        }
    }
}
