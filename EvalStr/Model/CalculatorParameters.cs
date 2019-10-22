using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvalStr.Model
{
    public class CalculatorResponse
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public string Result { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class CalculatorRequest
    {
        public string Expression { get; set; }
    }
}
