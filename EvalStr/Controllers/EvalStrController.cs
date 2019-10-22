using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EvalStr.Model;
using EvalStr.Workers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EvalStr.Controllers
{
    [ApiController]
    public class EvalStrController : ControllerBase
    {
        private readonly ILogger<EvalStrController> _logger;

        public EvalStrController(ILogger<EvalStrController> logger)
        {
            _logger = logger;
        }

        [Route("evalstr")]
        [HttpGet, HttpPost]
        [EnableCors("CorsPolicy")]
        public async Task<HttpResponseMessage> Get()
        {
            string content = string.Empty;
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();

                _logger.LogInformation($"body: {body}");
                content = body;
            }
            if (!string.IsNullOrWhiteSpace(content))
            {
                float value;
                string errMsg;
                if (EvalStrWorker.EvalStr(content, out value, out errMsg))
                {
                    _logger.LogInformation($"value: {value}");
                    return new HttpResponseMessage
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = new StringContent(value.ToString())
                    };
                }
                else
                {
                    _logger.LogInformation($"errMsg: {errMsg}");
                    return new HttpResponseMessage
                    {
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Content = new StringContent(errMsg)
                    };
                }
            }
            return new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Content = new StringContent("Expression can't be null.")
            };
        }

        [Route("evalstr/json")]
        [HttpPost]
        [EnableCors("CorsPolicy")]
        async public Task<CalculatorResponse> Post(CalculatorRequest exp)
        {
            string content = exp.Expression;
            if (!string.IsNullOrWhiteSpace(content))
            {
                float value;
                string errMsg;
                if (EvalStrWorker.EvalStr(content, out value, out errMsg))
                {
                    _logger.LogInformation($"value: {value}");
                    return new CalculatorResponse
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Result = value.ToString()
                    };
                }
                else
                {
                    _logger.LogInformation($"errMsg: {errMsg}");
                    return new CalculatorResponse
                    {
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        ErrorMessage = errMsg
                    };
                }
            }
            return new CalculatorResponse
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                ErrorMessage = "Expression can't be null."
            };
        }
    }
}
