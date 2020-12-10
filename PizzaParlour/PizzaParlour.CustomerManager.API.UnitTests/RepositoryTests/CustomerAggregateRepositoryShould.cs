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
    public class CustomerAggregateRepositoryShould
    {
        private Mock<CosmosClient> _cosmosClientMock;
        private Mock<IConfiguration> _configMock;
        private Mock<Container> _customerAggContainerMock;

        private CustomerAggregateRepository _sut;

        public CustomerAggregateRepositoryShould()
        {
            _cosmosClientMock = new Mock<CosmosClient>();
            _customerAggContainerMock = new Mock<Container>();
            _cosmosClientMock.Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(_customerAggContainerMock.Object);
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(c => c["DatabaseName"]).Returns("dbname");
            _configMock.Setup(c => c["ContainerName"]).Returns("containername");

            _sut = new CustomerAggregateRepository(
                _cosmosClientMock.Object,
                _configMock.Object);
        }

        [Fact]
        public async Task FireDeleteItemAsyncSuccessfully()
        {
            // Arrange
            var testCustomer = TestDataGenerator.GenerateCustomer();

            _customerAggContainerMock.SetupDeleteItemAsync<Customer>();

            // Act
            await _sut.DeleteCustomer(testCustomer.CustomerId);

            // Assert
            _customerAggContainerMock.Verify(c => c.DeleteItemAsync<Customer>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task FireReadItemAsyncSuccessfully()
        {
            // Arrange
            var testCustomer = TestDataGenerator.GenerateCustomer();

            _customerAggContainerMock.SetupReadItemAsync(testCustomer);

            // Act
            var response = await _sut.GetCustomerById(testCustomer.CustomerId);

            // Assert
            _customerAggContainerMock.Verify(c => c.ReadItemAsync<Customer>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal(testCustomer.CustomerId, response.CustomerId);
            Assert.Equal(testCustomer.Name, response.Name);
            Assert.Equal(testCustomer.Email, response.Email);
            Assert.Equal(testCustomer.Password, response.Password);
            Assert.Equal(testCustomer.PhoneNumber, response.PhoneNumber);
            Assert.Equal(testCustomer.Address.AddressLine1, response.Address.AddressLine1);
            Assert.Equal(testCustomer.Address.AddressLine2, response.Address.AddressLine2);
            Assert.Equal(testCustomer.Address.City, response.Address.City);
            Assert.Equal(testCustomer.Address.State, response.Address.State);
            Assert.Equal(testCustomer.Address.ZipCode, response.Address.ZipCode);
        }

    }
}
