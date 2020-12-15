using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PizzaParlour.OrderManager.API.Repositories;

namespace PizzaParlour.OrderManager.API.Functions
{
    public class GetAllOrdersByCustomerId
    {
        private readonly ILogger<GetAllOrdersByCustomerId> _logger;
        private readonly IOrderAggregateRepository _orderAggregateRepository;

        public GetAllOrdersByCustomerId(
            ILogger<GetAllOrdersByCustomerId> logger,
            IOrderAggregateRepository orderAggregateRepository)
        {
            _logger = logger;
            _orderAggregateRepository = orderAggregateRepository;
        }

        [FunctionName(nameof(GetAllOrdersByCustomerId))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Orders/{customerId}")] HttpRequest req,
            string customerId)
        {
            IActionResult result = null;

            try
            {
                if (string.IsNullOrEmpty(customerId))
                {
                    _logger.LogError($"Customer Id is not specified");
                    result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                }
                else
                {
                    var orders = await _orderAggregateRepository.GetAllOrdersByCustomerId(customerId);
                    result = new OkObjectResult(orders);
                }
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
