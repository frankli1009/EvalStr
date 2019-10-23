using EvalStr.Interface;
using EvalStr.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvalStr.Workers
{
    public static class EvalStrWorker
    {
        public static bool EvalStr(string s, out float value, out string errMsg)
        {
            int endIndex;
            value = 0;
            ArithmeticExpression<ICalculator> expression = ParseExpression(s, out endIndex, out errMsg);
            if (expression != null && string.IsNullOrEmpty(errMsg))
            {
                value = expression.GetValue().Value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parse string expression to an arithmetic expression object
        /// </summary>
        /// <param name="s"></param>
        /// <param name="endIndex"></param>
        /// <param name="errMsg"></param>
        /// <param name="startIndex"></param>
        /// <param name="recurLevel"></param>
        /// <returns></returns>
        private static ArithmeticExpression<ICalculator> ParseExpression(string s, out int endIndex, out string errMsg, int startIndex = 0, int recurLevel = 0)
        {
            endIndex = startIndex;
            errMsg = string.Empty;
            ArithmeticExpression<ICalculator> root = null;

            if (!string.IsNullOrWhiteSpace(s) && s.Length > startIndex)
            {
                int index = startIndex;
                ArithmeticExpression<ICalculator> prevExpression = null;
                ArithmeticExpression<ICalculator> curExpression = null;
                ArithmeticOperand<ICalculator> newOperand = null;

                bool prevOperand = false;
                bool prevOperator = false;

                while (index < s.Length)
                {
                    char c = s[index];
                    // Check operator
                    if (c == '+' || c == '-' || c == '*' || c == '/')
                    {
                        if (curExpression != null)
                        {
                            prevExpression = curExpression;
                        }
                        curExpression = ArithmeticExpressionFactory.CreateArithmeticExpression(c, index, ref prevOperator, ref prevOperand, ref errMsg);
                        if (curExpression == null)
                        {
                            break;
                        }
                        else if (prevExpression != null)
                        {
                            if (!ArithmeticExpressionFactory.MergeExpressions(prevExpression, ref curExpression, ref newOperand, ref errMsg))
                            {
                                break;
                            }
                        }
                        else
                        {
                            prevExpression = curExpression;
                            if (newOperand != null)
                            {
                                curExpression.AddOperand(newOperand);
                                newOperand = null;
                            }
                        }
                    }
                    // Check number
                    else if (c == '(')
                    {
                        int childEndIndex;
                        string childErrMsg;
                        ArithmeticExpression<ICalculator> childExpression = ParseExpression(s, out childEndIndex, out childErrMsg, index + 1, recurLevel + 1);
                        if (!string.IsNullOrWhiteSpace(childErrMsg))
                        {
                            errMsg += childErrMsg;
                            break;
                        }
                        else if (childExpression == null)
                        {
                            errMsg += $"Expression missing in parentheses: [index: {index}].";
                            break;
                        }
                        else if (prevOperand)
                        {
                            errMsg += $"Operator is missing before character: ( [index: {index}].";
                            break;
                        }
                        else
                        {
                            index = childEndIndex; // ')'
                            newOperand = new ArithmeticOperand<ICalculator>
                            {
                                Expression = childExpression
                            };
                            prevOperand = true;
                            prevOperator = false;
                        }
                    }
                    else if (c == ')')
                    {
                        recurLevel--;
                        if (recurLevel < 0)
                        {
                            errMsg += $"Redundant right parenthesis: [index: {index}].";
                        }
                        break;
                    }
                    else if (c >= '0' && c <= '9')
                    {
                        IntCalculator intCalculator = ArithmeticExpressionFactory.CreateCalculator(s, ref index, ref prevOperator, ref prevOperand, ref errMsg) as IntCalculator;
                        if (intCalculator == null)
                        {
                            break;
                        }
                        else
                        {
                            newOperand = new ArithmeticOperand<ICalculator>
                            {
                                SimpleValue = intCalculator
                            };
                        }
                    }
                    // Support spaces
                    else if (c == ' ')
                    {

                    }
                    // Not support characters
                    else
                    {
                        errMsg += $"Illegal characters in the expression: {c} [index: {index}].";
                        break;
                    }

                    if (root == null && curExpression != null)
                    {
                        root = curExpression;
                    }

                    index++;
                }

                if (index == s.Length && recurLevel > 0)
                {
                    errMsg += $"Missing {recurLevel} right parenthesis.";
                }
                else if (newOperand != null && curExpression != null)
                {
                    curExpression.AddOperand(newOperand);
                    newOperand = null;
                }

                endIndex = index;
            }

            if (!string.IsNullOrEmpty(errMsg))
            {
                root = null;
            }

            if (root != null)
            {
                while (root.ParentExpression != null)
                {
                    root = root.ParentExpression;
                }
            }
            return root;
        }
    }
}
