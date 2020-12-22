using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PizzaParlour.OrderManager.API.Functions;
using PizzaParlour.OrderManager.API.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
