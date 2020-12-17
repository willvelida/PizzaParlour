using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Moq;
using PizzaParlour.Core.Models;
using PizzaParlour.OrderManager.API.Repositories;
using PizzaParlour.OrderManager.API.UnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PizzaParlour.OrderManager.API.UnitTests.RepositoryTests
{
    public class OrderAggregateRepositoryShould
    {
        private Mock<CosmosClient> _cosmosClientMock;
        private Mock<IConfiguration> _configMock;
        private Mock<Container> _orderAggregateMock;

        private OrderAggregateRepository _sut;

        public OrderAggregateRepositoryShould()
        {
            _cosmosClientMock = new Mock<CosmosClient>();
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(x => x["DatabaseName"]).Returns("dbname");
            _configMock.Setup(x => x["OrderAggregateContainer"]).Returns("containername");
            _orderAggregateMock = new Mock<Container>();
            _cosmosClientMock.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(_orderAggregateMock.Object);

            _sut = new OrderAggregateRepository(
                _cosmosClientMock.Object,
                _configMock.Object);
        }

        [Fact]
        public async Task FireReadItemAsync()
        {
            // Arrange
            var testOrder = TestDataGenerator.GenerateOrder();

            _orderAggregateMock.SetupReadItemAsync(testOrder);

            // Act
            var response = await _sut.GetAnOrderByCustomerId(testOrder.CustomerId, testOrder.OrderId);

            // Assert
            _orderAggregateMock.Verify(c => c.ReadItemAsync<Order>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal(testOrder.OrderId, response.OrderId);
            Assert.Equal(testOrder.CustomerId, response.CustomerId);
            Assert.Equal(testOrder.OrderDate, response.OrderDate);
            Assert.Equal(testOrder.Items.First().LineCost, response.Items.First().LineCost);
            Assert.Equal(testOrder.Items.First().LineQuantity, response.Items.First().LineQuantity);
            Assert.Equal(testOrder.Items.First().LineItem, response.Items.First().LineItem);
            Assert.Equal(testOrder.DeliveryMethod, response.DeliveryMethod);
            Assert.Equal(testOrder.OrderAddress.AddressLine1, response.OrderAddress.AddressLine1);
            Assert.Equal(testOrder.OrderAddress.AddressLine2, response.OrderAddress.AddressLine2);
            Assert.Equal(testOrder.OrderAddress.City, response.OrderAddress.City);
            Assert.Equal(testOrder.OrderAddress.State, response.OrderAddress.State);
            Assert.Equal(testOrder.OrderAddress.ZipCode, response.OrderAddress.ZipCode);
            Assert.Equal(testOrder.PaymentMethod, response.PaymentMethod);
            Assert.Equal(response.OrderStatus, response.OrderStatus);
        }

        [Fact]
        public async Task FireGetItemQueryIterator()
        {
            // Arrange
            var orders = new List<Order>();
            var testOrder = TestDataGenerator.GenerateOrder();
            orders.Add(testOrder);

            _orderAggregateMock.SetupItemQueryIteratorMock(orders);
            _orderAggregateMock.SetupItemQueryIteratorMock(new List<int> { 1 });

            // Act
            var response = await _sut.GetAllOrdersByCustomerId(testOrder.CustomerId);

            // Assert
            foreach (var order in orders)
            {
                Assert.Equal(testOrder.CustomerId, order.CustomerId);
            }
        }

        [Fact]
        public async Task FireDeleteItemAsync()
        {
            // Arrange
            var testOrder = TestDataGenerator.GenerateOrder();

            _orderAggregateMock.SetupDeleteItemAsync<Order>();

            // Act
            await _sut.DeleteOrder(testOrder.CustomerId, testOrder.OrderId);

            // Assert
            _orderAggregateMock.Verify(c => c.DeleteItemAsync<Order>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
