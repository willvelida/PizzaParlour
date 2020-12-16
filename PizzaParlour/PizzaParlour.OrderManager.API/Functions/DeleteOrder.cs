using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PizzaParlour.OrderManager.API.Repositories;
using Microsoft.Azure.Cosmos;

namespace PizzaParlour.OrderManager.API.Functions
{
    public class DeleteOrder
    {
        private readonly ILogger<DeleteOrder> _logger;
        private readonly IOrderAggregateRepository _orderAggregateRepository;

        public DeleteOrder(
            ILogger<DeleteOrder> logger,
            IOrderAggregateRepository orderAggregateRepository)
        {
            _logger = logger;
            _orderAggregateRepository = orderAggregateRepository;
        }

        [FunctionName(nameof(DeleteOrder))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Order/{customerId}/{orderId}")] HttpRequest req,
            string customerId,
            string orderId)
        {
            IActionResult result = null;

            try
            {
                if (string.IsNullOrEmpty(customerId))
                {
                    _logger.LogError($"Customer Id is not supplied");
                    result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                }

                if (string.IsNullOrEmpty(orderId))
                {
                    _logger.LogError($"Order Id is not supplied");
                    result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                }

                await _orderAggregateRepository.DeleteOrder(customerId, orderId);
                result = new StatusCodeResult(StatusCodes.Status200OK);
            }
            catch (CosmosException cex) when (cex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogError($"Could not find Order Id: {orderId} for Customer Id: {customerId}");
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
