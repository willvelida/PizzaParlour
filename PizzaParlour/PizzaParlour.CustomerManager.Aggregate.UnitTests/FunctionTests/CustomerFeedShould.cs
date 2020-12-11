using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using PizzaParlour.Core.Models;
using PizzaParlour.CustomerManager.Aggregate.Functions;
using PizzaParlour.CustomerManager.Aggregate.Repositories;
using PizzaParlour.CustomerManager.Aggregate.UnitTests.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace PizzaParlour.CustomerManager.Aggregate.UnitTests.FunctionTests
{
    public class CustomerFeedShould
    {
        private Mock<ILogger<CustomerFeed>> _loggerMock;
        private Mock<ICustomerAggregateRepository> _customerAggregateRepoMock;

        private CustomerFeed _func;

        public CustomerFeedShould()
        {
            _loggerMock = new Mock<ILogger<CustomerFeed>>();
            _customerAggregateRepoMock = new Mock<ICustomerAggregateRepository>();

            _func = new CustomerFeed(
                _loggerMock.Object,
                _customerAggregateRepoMock.Object);
        }

        [Fact]
        public async Task UpsertNewDocument()
        {
            // Arrange
            var documentList = new List<Document>();
            var testCustomer = TestDataGenerator.GenerateCustomer();
            var customerDocument = ConvertCustomerObjectToDocument(testCustomer);
            documentList.Add(customerDocument);

            // Act
            await _func.Run(documentList);

            // Assert
            _customerAggregateRepoMock.Verify(
                r => r.UpsertCustomer(It.IsAny<Customer>()), Times.Once);
        }

        private Document ConvertCustomerObjectToDocument(Customer customer)
        {
            var customerJSON = JsonConvert.SerializeObject(customer);
            var document = new Document();
            document.LoadFrom(new JsonTextReader(new StringReader(customerJSON)));

            return document;
        }
    }
}
