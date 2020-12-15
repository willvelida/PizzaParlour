using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using PizzaParlour.OrderManager.API.Repositories;
using Microsoft.Azure.Cosmos;

namespace PizzaParlour.OrderManager.API.Functions
{
    public class GetOrderByCustomerId
    {
        private readonly ILogger<GetAllOrdersByCustomerId> _logger;
        private readonly IConfiguration _config;
        private readonly IOrderAggregateRepository _orderAggregateRepository;

        public GetOrderByCustomerId(
            ILogger<GetAllOrdersByCustomerId> logger,
            IConfiguration config,
            IOrderAggregateRepository orderAggregateRepository)
        {
            _logger = logger;
            _config = config;
            _orderAggregateRepository = orderAggregateRepository;
        }

        [FunctionName(nameof(GetOrderByCustomerId))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Order/{customerId}/{orderId}")] HttpRequest req,
            string customerId,
            string orderId)
        {
            IActionResult result = null;

            try
            {
                if (string.IsNullOrEmpty(customerId))
                {
                    _logger.LogError("Customer ID is not specified");
                    result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                }

                if (string.IsNullOrEmpty(orderId))
                {
                    _logger.LogError("Order ID is not specified");
                    result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                }

                var order = await _orderAggregateRepository.GetAnOrderByCustomerId(customerId, orderId);

                result = new OkObjectResult(order);
            }
            catch (CosmosException cx) when (cx.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogError($"Order ID {orderId} for Customer ID {customerId} was not found!");
                result = new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error. Exception thrown: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
