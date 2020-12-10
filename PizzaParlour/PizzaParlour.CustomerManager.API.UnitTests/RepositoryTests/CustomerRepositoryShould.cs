using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Moq;
using PizzaParlour.Core.Models;
using PizzaParlour.CustomerManager.API.Repositories;
using PizzaParlour.CustomerManager.API.UnitTests.Helpers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PizzaParlour.CustomerManager.API.UnitTests.RepositoryTests
{
    public class CustomerRepositoryShould
    {
        private Mock<CosmosClient> _cosmosClientMock;
        private Mock<IConfiguration> _configMock;
        private Mock<Container> _customerAggContainerMock;

        private CustomerRepository _sut;

        public CustomerRepositoryShould()
        {
            _cosmosClientMock = new Mock<CosmosClient>();
            _customerAggContainerMock = new Mock<Container>();
            _cosmosClientMock.Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(_customerAggContainerMock.Object);
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(c => c["DatabaseName"]).Returns("dbname");
            _configMock.Setup(c => c["ContainerName"]).Returns("containername");

            _sut = new CustomerRepository(
                _cosmosClientMock.Object,
                _configMock.Object);
        }

        [Fact]
        public async Task FireCreateItemAsyncSuccessfully()
        {
            // Arrange
            var testCustomer = TestDataGenerator.GenerateCustomer();
            _customerAggContainerMock.SetupCreateItemAsync<Customer>();

            // Act
            await _sut.AddCustomer(testCustomer);

            // Assert
            _customerAggContainerMock.Verify(c => c.CreateItemAsync(
                It.IsAny<Customer>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
