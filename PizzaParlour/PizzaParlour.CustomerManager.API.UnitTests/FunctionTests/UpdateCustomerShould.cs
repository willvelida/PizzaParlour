using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using PizzaParlour.Core.Models;
using PizzaParlour.CustomerManager.API.Functions;
using PizzaParlour.CustomerManager.API.Repositories;
using PizzaParlour.CustomerManager.API.UnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaParlour.CustomerManager.API.UnitTests.FunctionTests
{
    public class UpdateCustomerShould
    {
        private Mock<ILogger<UpdateCustomer>> _loggerMock;
        private Mock<ICustomerRepository> _customerRepoMock;
        private Mock<ICustomerAggregateRepository> _customerAggregateRepoMock;
        private Mock<HttpRequest> _httpRequestMock;

        private UpdateCustomer _func;

        public UpdateCustomerShould()
        {
            _loggerMock = new Mock<ILogger<UpdateCustomer>>();
            _customerRepoMock = new Mock<ICustomerRepository>();
            _customerAggregateRepoMock = new Mock<ICustomerAggregateRepository>();
            _httpRequestMock = new Mock<HttpRequest>();

            _func = new UpdateCustomer(
                _loggerMock.Object,
                _customerAggregateRepoMock.Object,
                _customerRepoMock.Object);
        }

        [Fact]
        public async Task UpdateCustomerSuccessfully()
        {
            // Arrange
            var customer = TestDataGenerator.GenerateCustomer();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(customer));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _customerAggregateRepoMock.Setup(c => c.GetCustomerById(It.IsAny<string>())).ReturnsAsync(customer);
            _customerRepoMock.Setup(c => c.AddCustomer(It.IsAny<Customer>())).Returns(Task.CompletedTask);

            // Act
            var response = await _func.Run(_httpRequestMock.Object, customer.CustomerId);

            // Assert
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseAsStatusCode = (StatusCodeResult)response;
            Assert.Equal(200, responseAsStatusCode.StatusCode);
        }

        [Fact]
        public async Task Throw400OnBadRequest()
        {
            // Arrange
            var customer = new Customer();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(customer));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            // Act
            var response = await _func.Run(_httpRequestMock.Object, null);

            // Assert
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseAsStatusCode = (StatusCodeResult)response;
            Assert.Equal(400, responseAsStatusCode.StatusCode);
        }

        [Fact]
        public async Task Throw404OnNotFound()
        {
            // Arrange
            var customer = TestDataGenerator.GenerateCustomer();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(customer));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            double fakeRU = 0.0;

            _customerAggregateRepoMock.Setup(s => s.GetCustomerById(It.IsAny<string>()))
                .Throws(new CosmosException("Not found", System.Net.HttpStatusCode.NotFound, 404, "someActivity", fakeRU));

            // Act
            var response = await _func.Run(_httpRequestMock.Object, customer.CustomerId);

            // Assert
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseAsStatusCode = (StatusCodeResult)response;
            Assert.Equal(404, responseAsStatusCode.StatusCode);
        }

        [Fact]
        public async Task Throw500ForInternalServerError()
        {
            // Arrange
            var customer = TestDataGenerator.GenerateCustomer();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(customer));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _customerAggregateRepoMock.Setup(s => s.GetCustomerById(It.IsAny<string>())).Throws(new Exception("some error"));

            // Act
            var response = await _func.Run(_httpRequestMock.Object, customer.CustomerId);

            // Assert
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseAsStatusCode = (StatusCodeResult)response;
            Assert.Equal(500, responseAsStatusCode.StatusCode);
        }
    }
}
