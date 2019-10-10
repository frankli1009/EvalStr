using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvalStr.Interface
{
    public interface ICalculator
    {
        public float Value { get; set; }

        ICalculator Add(ICalculator op);
        ICalculator Subtract(ICalculator op);
        ICalculator Multiply(ICalculator op);
        ICalculator Divide(ICalculator op);
    }
}
