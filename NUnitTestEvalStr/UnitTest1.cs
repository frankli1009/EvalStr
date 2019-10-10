using EvalStr.Controllers;
using EvalStr.Workers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

namespace NUnitTestEvalStr
{
    public class Tests
    {
        private ILogger<EvalStrController> _logger;
        private EvalStrController _evalStrController;

        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<ILogger<EvalStrController>>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            loggerFactory.CreateLogger(Arg.Any<string>()).Returns(_logger);

            _evalStrController = new EvalStrController(_logger);
        }

        [Test]
        public void Test1()
        {
            string content = "4 + 5  *2";
            float value;
            string errMsg;
            bool result = EvalStrWorker.EvalStr(content, out value, out errMsg);
            Assert.IsTrue(result);
            Assert.AreEqual(14, value);
            Assert.IsTrue(string.IsNullOrEmpty(errMsg));
        }

        [Test]
        public void Test2()
        {
            string content = "4+ 5 /2";
            float value;
            string errMsg;
            bool result = EvalStrWorker.EvalStr(content, out value, out errMsg);
            Assert.IsTrue(result);
            Assert.AreEqual(6.5, value);
            Assert.IsTrue(string.IsNullOrEmpty(errMsg));
        }

        [Test]
        public void Test3()
        {
            string content = " 4 + 5  /2-1";
            float value;
            string errMsg;
            bool result = EvalStrWorker.EvalStr(content, out value, out errMsg);
            Assert.IsTrue(result);
            Assert.AreEqual(5.5, value);
            Assert.IsTrue(string.IsNullOrEmpty(errMsg));
        }

        [Test]
        public void Test4()
        {
            string content = "4 + 5.2  *2";
            float value;
            string errMsg;
            bool result = EvalStrWorker.EvalStr(content, out value, out errMsg);
            Assert.IsTrue(!result);
            Assert.IsTrue(!string.IsNullOrEmpty(errMsg));
        }

        [Test]
        public async Task Test5()
        {
            // Arrange
            string content = "4 + 5  *2";
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(content);
            sw.Flush();
            ms.Position = 0;

            var httpContext = new DefaultHttpContext(); // or mock a `HttpContext`
            //httpContext.Request.Headers["token"] = "fake_token_here"; //Set header
            httpContext.Request.Body = ms; 
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            _evalStrController.ControllerContext = controllerContext;

            // Act
            var result = await _evalStrController.Get();

            // Assert
            string value = await result.Content.ReadAsStringAsync();
            Assert.AreEqual("14", value);
            Assert.IsTrue(result.IsSuccessStatusCode);
        }
    }
}