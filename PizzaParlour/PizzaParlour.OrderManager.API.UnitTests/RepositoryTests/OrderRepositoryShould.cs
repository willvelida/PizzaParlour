using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Moq;
using PizzaParlour.OrderManager.API.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaParlour.OrderManager.API.UnitTests.RepositoryTests
{
    public class OrderRepositoryShould
    {
        private Mock<CosmosClient> _cosmosClientMock;
        private Mock<IConfiguration> _configMock;
        private Mock<Container> _orderContainerMock;

        private OrderRepository _sut;

        public OrderRepositoryShould()
        {
            _cosmosClientMock = new Mock<CosmosClient>();
            _orderContainerMock = new Mock<Container>();
            _cosmosClientMock.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(_orderContainerMock.Object);
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(x => x["DatabaseName"]).Returns("dbname");
            _configMock.Setup(x => x["OrderContainerName"]).Returns("container");

            _sut = new OrderRepository(
                _cosmosClientMock.Object,
                _configMock.Object);
        }
    }
}
