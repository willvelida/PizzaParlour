using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using PizzaParlour.Core.Models;
using PizzaParlour.OrderManager.API.Functions;
using PizzaParlour.OrderManager.API.Repositories;
using PizzaParlour.OrderManager.API.UnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaParlour.OrderManager.API.UnitTests.FunctionTests
{
    public class CreateOrderShould
    {
        private Mock<ILogger<CreateOrder>> _loggerMock;
        private Mock<IConfiguration> _configMock;
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<HttpRequest> _httpRequestMock;

        private CreateOrder _func;

        public CreateOrderShould()
        {
            _loggerMock = new Mock<ILogger<CreateOrder>>();
            _configMock = new Mock<IConfiguration>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _httpRequestMock = new Mock<HttpRequest>();

            _func = new CreateOrder(
                _loggerMock.Object,
                _configMock.Object,
                _orderRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateOrderSuccessfully()
        {
            // Arrange
            var order = TestDataGenerator.GenerateOrder();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(order));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _orderRepositoryMock.Setup(s => s.AddOrder(It.IsAny<Order>())).Returns(Task.CompletedTask);

            // Act
            var response = await _func.Run(_httpRequestMock.Object);

            // Assert
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseAsStatusCode = (StatusCodeResult)response;
            Assert.Equal(201, responseAsStatusCode.StatusCode);
        }

        [Fact]
        public async Task Throw500OnInternalServerError()
        {
            // Arrange
            var order = TestDataGenerator.GenerateOrder();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(order));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _orderRepositoryMock.Setup(s => s.AddOrder(It.IsAny<Order>())).Throws(new Exception("Some error"));

            // Act
            var response = await _func.Run(_httpRequestMock.Object);

            // Assert
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseAsStatusCode = (StatusCodeResult)response;
            Assert.Equal(500, responseAsStatusCode.StatusCode);
        }
    }
}
