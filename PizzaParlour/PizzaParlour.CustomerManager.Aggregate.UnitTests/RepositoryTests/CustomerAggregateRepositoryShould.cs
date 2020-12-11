using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Moq;
using PizzaParlour.Core.Models;
using PizzaParlour.CustomerManager.Aggregate.Repositories;
using PizzaParlour.CustomerManager.Aggregate.UnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PizzaParlour.CustomerManager.Aggregate.UnitTests.RepositoryTests
{
    public class CustomerAggregateRepositoryShould
    {
        private Mock<CosmosClient> _cosmosMock;
        private Mock<IConfiguration> _configMock;
        private Mock<Container> _customerAggregateContainerMock;

        private CustomerAggregateRepository _sut;

        public CustomerAggregateRepositoryShould()
        {
            _cosmosMock = new Mock<CosmosClient>();
            _configMock = new Mock<IConfiguration>();
            _customerAggregateContainerMock = new Mock<Container>();

            _cosmosMock.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(_customerAggregateContainerMock.Object);
            _configMock.Setup(x => x["DatabaseName"]).Returns("dbname");
            _configMock.Setup(x => x["CustomerAggregateContainer"]).Returns("container");

            _sut = new CustomerAggregateRepository(
                _cosmosMock.Object,
                _configMock.Object);
        }

        [Fact]
        public async Task FireUpsertItemAsync()
        {
            // Arrange
            var testCustomer = TestDataGenerator.GenerateCustomer();

            _customerAggregateContainerMock.SetupUpsertItemAsync<Customer>();

            // Act
            await _sut.UpsertCustomer(testCustomer);

            // Assert
            _customerAggregateContainerMock.Verify(c => c.UpsertItemAsync<Customer>(
                It.IsAny<Customer>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
