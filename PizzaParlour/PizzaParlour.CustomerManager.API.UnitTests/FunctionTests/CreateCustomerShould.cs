using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class CreateCustomerShould
    {
        private Mock<ILogger<CreateCustomer>> _loggerMock;
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private Mock<HttpRequest> _httpRequestMock;

        private CreateCustomer _func;

        public CreateCustomerShould()
        {
            _loggerMock = new Mock<ILogger<CreateCustomer>>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _httpRequestMock = new Mock<HttpRequest>();

            _func = new CreateCustomer(
                _loggerMock.Object,
                _customerRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateCustomerSuccessfully()
        {
            // Arrange
            var customer = TestDataGenerator.GenerateCustomer();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(customer));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _customerRepositoryMock.Setup(s => s.AddCustomer(It.IsAny<Customer>())).Returns(Task.CompletedTask);

            // Act
            var response = await _func.Run(_httpRequestMock.Object);

            // Assert
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseAsStatusCode = (StatusCodeResult)response;
            Assert.Equal(201, responseAsStatusCode.StatusCode);
        }

        [Fact]
        public async Task Throw500ForInternalServerErrors()
        {
            // Arrange
            var customer = TestDataGenerator.GenerateCustomer();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(customer));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _customerRepositoryMock.Setup(s => s.AddCustomer(It.IsAny<Customer>())).Throws(new Exception("Some error"));

            // Act
            var response = await _func.Run(_httpRequestMock.Object);

            // Assert
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseAsStatusCode = (StatusCodeResult)response;
            Assert.Equal(500, responseAsStatusCode.StatusCode);
        }
    }
}
