using EvalStr.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvalStr.Model
{
    public class IntCalculator : ICalculator
    {
        private float _value = 0;

        public ICalculator Add(ICalculator op)
        {
            if (op != null)
            {
                return new IntCalculator
                {
                    Value = this.Value + op.Value
                };
            }
            else
            {
                return Copy();
            }
        }

        public ICalculator Divide(ICalculator op)
        {
            if (op != null)
            {
                return new IntCalculator
                {
                    Value = this.Value / op.Value
                };
            }
            else
            {
                return Copy();
            }
        }

        public ICalculator Multiply(ICalculator op)
        {
            if (op != null)
            {
                return new IntCalculator
                {
                    Value = this.Value * op.Value
                };
            }
            else
            {
                return Copy();
            }
        }

        public ICalculator Subtract(ICalculator op)
        {
            if (op != null)
            {
                return new IntCalculator
                {
                    Value = this.Value - op.Value
                };
            }
            else
            {
                return Copy();
            }
        }

        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public IntCalculator Copy()
        {
            return new IntCalculator
            {
                Value = this.Value
            };
        }
    }
}
