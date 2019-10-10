using EvalStr.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvalStr.Model
{
    public class ArithmeticOperator
    {
        private OperatorType _operator = OperatorType.None;

        public OperatorType Operator
        {
            get
            {
                return _operator;
            }
            set
            {
                _operator = value;
            }
        }

        public static ArithmeticOperator FromChar(char op)
        {
            var arithmeticOp = new ArithmeticOperator();
            arithmeticOp = arithmeticOp.FromChar(op);
            return arithmeticOp;
        }
    }
}
