using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using PizzaParlour.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaParlour.OrderManager.API.Repositories
{
    public class OrderAggregateRepository : IOrderAggregateRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IConfiguration _config;
        private Container _orderAggregateContainer;

        public OrderAggregateRepository(
            CosmosClient cosmosClient,
            IConfiguration config)
        {
            _cosmosClient = cosmosClient;
            _config = config;

            _orderAggregateContainer = _cosmosClient.GetContainer(_config["DatabaseName"], _config["OrderAggregateContainer"]);
        }

        public async Task DeleteOrder(string customerId, string orderId)
        {
            ItemRequestOptions itemRequestOptions = new ItemRequestOptions
            {
                EnableContentResponseOnWrite = false
            };

            await _orderAggregateContainer.DeleteItemAsync<Order>(
                orderId,
                new PartitionKey(customerId),
                itemRequestOptions);
        }

        public async Task<List<Order>> GetAllOrdersByCustomerId(string customerId)
        {
            var orders = new List<Order>();

            QueryDefinition query = new QueryDefinition("SELECT * FROM OrderAggregate c WHERE c.CustomerId = @customerId")
                .WithParameter("@customerId", customerId);

            FeedIterator<Order> orderResults = _orderAggregateContainer.GetItemQueryIterator<Order>(
                query,
                requestOptions: new QueryRequestOptions()
                {
                    PartitionKey = new PartitionKey(customerId)
                });

            while (orderResults.HasMoreResults)
            {
                FeedResponse<Order> orderResponse = await orderResults.ReadNextAsync();
                orders.AddRange(orderResponse.Resource);
            }

            return orders;
        }

        public async Task<Order> GetAnOrderByCustomerId(string customerId, string orderId)
        {
            ItemResponse<Order> orderResponse = await _orderAggregateContainer.ReadItemAsync<Order>
                (
                    orderId,
                    new PartitionKey(customerId)
                );

            return orderResponse.Resource;
        }
    }
}
