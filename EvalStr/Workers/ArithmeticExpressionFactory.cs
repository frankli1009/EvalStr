using EvalStr.Extensions;
using EvalStr.Interface;
using EvalStr.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvalStr.Workers
{
    public static class ArithmeticExpressionFactory
    {
        /// <summary>
        /// Create an arithmetic expression
        /// </summary>
        /// <param name="op"></param>
        /// <param name="index"></param>
        /// <param name="prevOperator"></param>
        /// <param name="prevOperand"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static ArithmeticExpression<ICalculator> CreateArithmeticExpression(char op, int index, ref bool prevOperator, ref bool prevOperand, ref string errMsg)
        {
            ArithmeticExpression<ICalculator> curExpression = null;
            if (op == '+' || op == '-')
            {
                if (prevOperator)
                {
                    errMsg += $"Operand is missing before character: {op} [index={index}].";
                    return null;
                }
                curExpression = new AddSubtractExpression<ICalculator>();
                if (!prevOperand)
                {
                    curExpression.Operands.Add(new ArithmeticOperand<ICalculator>
                    {
                        SimpleValue = new IntCalculator
                        {
                            Value = 0
                        }
                    });
                }
            }
            else if (op == '*' || op == '/')
            {
                if (prevOperator || !prevOperand)
                {
                    errMsg += $"Operand is missing before character: {op} [index={index}].";
                    return null;
                }
                curExpression = new MultiplyDivideExpression<ICalculator>();
            }
            curExpression.Operators.Add(ArithmeticOperator.FromChar(op));

            prevOperator = true;
            prevOperand = false;
            return curExpression;
        }

        /// <summary>
        /// Create an number (ICalculator) for operand from the reading of integer number
        /// </summary>
        /// <param name="s"></param>
        /// <param name="index"></param>
        /// <param name="prevOperator"></param>
        /// <param name="prevOperand"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static ICalculator CreateCalculator(string s, ref int index, ref bool prevOperator, ref bool prevOperand, ref string errMsg)
        {
            if (string.IsNullOrEmpty(s) || index >= s.Length)
            {
                errMsg += $"Out of bound. Expression length: {s?.Length}, index: {index}.";
                return null;
            }
            char c = s[index];
            if (prevOperand)
            {
                errMsg += $"Operator is missing before character: {c} [index: {index}].";
                return null;
            }

            prevOperand = true;
            prevOperator = false;
            return getNumberExpression<int>(s, ref index, ref errMsg);
        }

        /// <summary>
        /// Get an number (ICalculator) for operand from the reading of number, support the extension of supporting float number
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="index"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        private static ICalculator getNumberExpression<T>(string s, ref int index, ref string errMsg)
        {
            int startIndex = index;

            string numS = string.Empty;
            while (index < s.Length)
            {
                char c = s[index];
                if (c >= '0' && c <= '9')
                {
                    numS += c;
                }
                else break;
                index++;
            }

            if (!string.IsNullOrEmpty(numS))
            {
                switch(typeof(T).Name)
                {
                    case "Int32":
                        index--;
                        int num = Convert.ToInt32(numS);
                        return new IntCalculator
                        {
                            Value = num
                        };
                    default:
                        errMsg += $"Unsupport data type {typeof(T).Name}.";
                        return null;
                }
            }
            else
            {
                errMsg += $"Failed to get number: {numS} [startIndex: {startIndex}, endIndex: {index}].";
                return null;
            }
        }

        /// <summary>
        /// Merge current expression with previous expression and the new operand just got before current expression
        /// </summary>
        /// <param name="prevExpression"></param>
        /// <param name="curExpression"></param>
        /// <param name="newOperand"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool MergeExpressions(ArithmeticExpression<ICalculator> prevExpression, ref ArithmeticExpression<ICalculator> curExpression, ref ArithmeticOperand<ICalculator> newOperand, ref string errMsg)
        {
            if (prevExpression == null || curExpression == null)
            {
                errMsg += "prevExpression is null or curExpression is null in MergeExpressions.";
                return false;
            }

            if (prevExpression.Priority() < curExpression.Priority())
            {
                curExpression.AddOperand(newOperand);
                prevExpression.AddOperand(curExpression);
            }
            else if (prevExpression.Priority() > curExpression.Priority())
            {
                prevExpression.AddOperand(newOperand);
                if (prevExpression.ParentExpression != null && prevExpression.ParentExpression.Priority() == curExpression.Priority())
                {
                    prevExpression.ParentExpression.Operators.AddRange(curExpression.Operators);
                    prevExpression.ParentExpression.AddOperands(curExpression.Operands);
                    curExpression = prevExpression.ParentExpression;
                }
                else
                {
                    curExpression.AddOperand(prevExpression);
                }
            }
            else
            {
                prevExpression.AddOperand(newOperand);
                prevExpression.Operators.AddRange(curExpression.Operators);
                prevExpression.AddOperands(curExpression.Operands);
                curExpression = prevExpression;
            }
            newOperand = null;

            return true;
        }
    }
}
