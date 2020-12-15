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
using PizzaParlour.Core.Models;

namespace PizzaParlour.OrderManager.API.Functions
{
    public class CreateOrder
    {
        private readonly ILogger<CreateOrder> _logger;
        private readonly IConfiguration _config;
        private readonly IOrderRepository _orderRepository;

        public CreateOrder(
            ILogger<CreateOrder> logger,
            IConfiguration config,
            IOrderRepository orderRepository)
        {
            _logger = logger;
            _config = config;
            _orderRepository = orderRepository;
        }

        [FunctionName(nameof(CreateOrder))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Order/{customerId}")] HttpRequest req)
        {
            IActionResult result = null;

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var order = JsonConvert.DeserializeObject<Order>(requestBody);

                await _orderRepository.AddOrder(order);

                result = new StatusCodeResult(StatusCodes.Status201Created);
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
