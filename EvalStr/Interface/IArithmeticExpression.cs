using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvalStr.Interface
{
    public interface IArithmeticExpression<T>
    {
        T GetValue();
    }
}
