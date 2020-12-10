using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using PizzaParlour.Core.Models;
using System;
using System.Threading.Tasks;

namespace PizzaParlour.CustomerManager.API.Repositories
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

        public async Task DeleteCustomer(string id)
        {
            ItemRequestOptions itemRequestOptions = new ItemRequestOptions()
            {
                EnableContentResponseOnWrite = false
            };

            await _customerAggregateContainer.DeleteItemAsync<Customer>(
                id,
                new PartitionKey(id),
                itemRequestOptions);
        }

        public async Task<Customer> GetCustomerById(string id)
        {
            ItemResponse<Customer> customerResponse = await _customerAggregateContainer.ReadItemAsync<Customer>(
                id,
                new PartitionKey(id));

            return customerResponse.Resource;
        }
    }
}
