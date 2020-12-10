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
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaParlour.CustomerManager.API.UnitTests.FunctionTests
{
    public class DeleteCustomerShould
    {
        private Mock<ILogger<DeleteCustomer>> _loggerMock;
        private Mock<ICustomerAggregateRepository> _customerAggregateRepositoryMock;
        private Mock<HttpRequest> _httpRequestMock;

        private DeleteCustomer _func;

        public DeleteCustomerShould()
        {
            _loggerMock = new Mock<ILogger<DeleteCustomer>>();
            _customerAggregateRepositoryMock = new Mock<ICustomerAggregateRepository>();
            _httpRequestMock = new Mock<HttpRequest>();

            _func = new DeleteCustomer(
                _loggerMock.Object,
                _customerAggregateRepositoryMock.Object);
        }

        [Fact]
        public async Task DeleteCustomerSuccessfully()
        {
            // Arrange
            var customer = TestDataGenerator.GenerateCustomer();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(customer));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _customerAggregateRepositoryMock.Setup(s => s.DeleteCustomer(It.IsAny<string>())).Returns(Task.CompletedTask);

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

            _customerAggregateRepositoryMock.Setup(s => s.DeleteCustomer(It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var response = await _func.Run(_httpRequestMock.Object, customer.CustomerId);

            // Assert
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseAsStatusCode = (StatusCodeResult)response;
            Assert.Equal(200, responseAsStatusCode.StatusCode);
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

            _customerAggregateRepositoryMock.Setup(s => s.DeleteCustomer(It.IsAny<string>()))
                .Throws(new CosmosException("Not found", System.Net.HttpStatusCode.NotFound, 404,"someActivity", fakeRU));

            // Act
            var response = await _func.Run(_httpRequestMock.Object, customer.CustomerId);

            // Assert
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseAsStatusCode = (StatusCodeResult)response;
            Assert.Equal(404, responseAsStatusCode.StatusCode);
        }

        [Fact]
        public async Task Throw500OnInternalServerError()
        {
            // Arrange
            var customer = TestDataGenerator.GenerateCustomer();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(customer));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _customerAggregateRepositoryMock.Setup(s => s.DeleteCustomer(It.IsAny<string>())).Throws(new Exception("some error"));

            // Act
            var response = await _func.Run(_httpRequestMock.Object, customer.CustomerId);

            // Assert
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseAsStatusCode = (StatusCodeResult)response;
            Assert.Equal(500, responseAsStatusCode.StatusCode);
        }
    }
}
