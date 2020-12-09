using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using PizzaParlour.Core.Models;
using System.Threading.Tasks;

namespace PizzaParlour.CustomerManager.Aggregate.Repositories
{
    public class CustomerAggregateRepository : ICustomerAggregateRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IConfiguration _config;
        private Container _customerAggregateContainer;

        public CustomerAggregateRepository(
            CosmosClient cosmosClient,
            IConfiguration config)
        {
            _cosmosClient = cosmosClient;
            _config = config;

            _customerAggregateContainer = _cosmosClient.GetContainer(_config["DatabaseName"], _config["CustomerAggregateContainer"]);
        }

        public async Task UpsertCustomer(Customer customer)
        {
            ItemRequestOptions itemRequestOptions = new ItemRequestOptions()
            {
                EnableContentResponseOnWrite = false
            };

            await _customerAggregateContainer.UpsertItemAsync(
                customer,
                new PartitionKey(customer.CustomerId),
                itemRequestOptions);
        }
    }
}
