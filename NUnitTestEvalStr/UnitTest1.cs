using EvalStr.Controllers;
using EvalStr.Model;
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
            // Arrange
            _logger = Substitute.For<ILogger<EvalStrController>>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            loggerFactory.CreateLogger(Arg.Any<string>()).Returns(_logger);

            _evalStrController = new EvalStrController(_logger);
        }

        [Test]
        public void Test1()
        {
            // Arrange
            string content = "4 + 5  *2";
            float value;
            string errMsg;

            // Act
            bool result = EvalStrWorker.EvalStr(content, out value, out errMsg);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(14, value);
            Assert.IsTrue(string.IsNullOrEmpty(errMsg));
        }

        [Test]
        public void Test2()
        {
            // Arrange
            string content = "4+ 5 /2";
            float value;
            string errMsg;

            // Act
            bool result = EvalStrWorker.EvalStr(content, out value, out errMsg);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(6.5, value);
            Assert.IsTrue(string.IsNullOrEmpty(errMsg));
        }

        [Test]
        public void Test3()
        {
            // Arrange
            string content = " 4 + 5  /2-1";
            float value;
            string errMsg;

            // Act
            bool result = EvalStrWorker.EvalStr(content, out value, out errMsg);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(5.5, value);
            Assert.IsTrue(string.IsNullOrEmpty(errMsg));
        }

        [Test]
        public void Test4()
        {
            // Arrange
            string content = "4 + 5.2  *2";
            float value;
            string errMsg;

            // Act
            bool result = EvalStrWorker.EvalStr(content, out value, out errMsg);

            // Assert
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

        [Test]
        public async Task Test6()
        {
            // Arrange
            string content = "3 + ((4 + 5)  *2 + 8/(5 - 1)) * 2";
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
            Assert.AreEqual("43", value);
            Assert.IsTrue(result.IsSuccessStatusCode);
        }

        [Test]
        public void Test7()
        {
            // Arrange
            string content = "4 + (5  *(2 + 3";
            float value;
            string errMsg;

            // Act
            bool result = EvalStrWorker.EvalStr(content, out value, out errMsg);

            // Assert
            Assert.IsTrue(!result);
            Assert.AreEqual("Missing 2 right parenthesis.", errMsg);
        }

        [Test]
        public void Test8()
        {
            // Arrange
            string content = "4 + 5  * -2";
            float value;
            string errMsg;

            // Act
            bool result = EvalStrWorker.EvalStr(content, out value, out errMsg);

            // Assert
            Assert.IsTrue(!result);
            Assert.AreEqual("Operand is missing before character: - [index=9].", errMsg);
        }

        [Test]
        public void Test9()
        {
            // Arrange
            string content = "4 + 5  * (-2)";
            float value;
            string errMsg;

            // Act
            bool result = EvalStrWorker.EvalStr(content, out value, out errMsg);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(-6, value);
            Assert.IsTrue(string.IsNullOrEmpty(errMsg));
        }

        [Test]
        public void Test10()
        {
            // Arrange
            string content = "4 + 1 3 / 2";
            float value;
            string errMsg;

            // Act
            bool result = EvalStrWorker.EvalStr(content, out value, out errMsg);

            // Assert
            Assert.IsTrue(!result);
            Assert.AreEqual("Operator is missing before character: 3 [index: 6].", errMsg);
        }

        [Test]
        public void Test11()
        {
            // Arrange
            string content = "-4 / 2 + (-2 + 4) / 2";
            float value;
            string errMsg;

            // Act
            bool result = EvalStrWorker.EvalStr(content, out value, out errMsg);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(-1, value);
            Assert.IsTrue(string.IsNullOrEmpty(errMsg));
        }

        [Test]
        public async Task Test12()
        {
            // Arrange
            CalculatorRequest request = new CalculatorRequest
            {
                Expression = "3 + ((4 + 5)  *2 + 8/(5 - 1)) * 2"
            };

            // Act
            var result = await _evalStrController.Post(request);

            // Assert
            Assert.AreEqual("43", result.Result);
            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.OK);
        }

        [Test]
        public async Task Test13()
        {
            // Arrange
            string content = "-4 (-2 + 4) / 2";
            float value;
            string errMsg;

            // Act
            bool result = EvalStrWorker.EvalStr(content, out value, out errMsg);

            // Assert
            Assert.IsTrue(!result);
            Assert.AreEqual("Operator is missing before character: ( [index: 3].", errMsg);
        }

        [Test]
        public async Task Test14()
        {
            // Act
            var result = await _evalStrController.GetList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("/evalstr/json", result[0].Path);
            Assert.AreEqual("/evalstr/json/float", result[1].Path);
        }
    }
}