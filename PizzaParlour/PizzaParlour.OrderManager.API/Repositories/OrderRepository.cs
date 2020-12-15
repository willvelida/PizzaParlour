using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using PizzaParlour.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PizzaParlour.OrderManager.API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IConfiguration _config;
        private Container _orderContainer;

        public OrderRepository(
            CosmosClient cosmosClient,
            IConfiguration config)
        {
            _cosmosClient = cosmosClient;
            _config = config;

            _orderContainer = _cosmosClient.GetContainer(_config["DatabaseName"], _config["OrderContainerName"]);
        }

        public async Task AddOrder(Order order)
        {
            ItemRequestOptions itemRequestOptions = new ItemRequestOptions()
            {
                EnableContentResponseOnWrite = false
            };

            await _orderContainer.CreateItemAsync(
                order,
                new PartitionKey(order.OrderId),
                itemRequestOptions);
        }
    }
}
