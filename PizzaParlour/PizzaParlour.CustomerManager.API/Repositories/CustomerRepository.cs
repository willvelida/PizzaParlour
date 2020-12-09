using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using PizzaParlour.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PizzaParlour.CustomerManager.API.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IConfiguration _config;
        private Container _customerContainer;

        public CustomerRepository(
            CosmosClient cosmosClient,
            IConfiguration config)
        {
            _cosmosClient = cosmosClient;
            _config = config;

            _customerContainer = _cosmosClient.GetContainer(_config["DatabaseName"], _config["ContainerName"]);
        }

        public async Task AddCustomer(Customer customer)
        {
            ItemRequestOptions itemRequestOptions = new ItemRequestOptions()
            {
                EnableContentResponseOnWrite = false
            };

            await _customerContainer.CreateItemAsync(
                customer,
                new PartitionKey(customer.CustomerId),
                itemRequestOptions);
        }
    }
}
