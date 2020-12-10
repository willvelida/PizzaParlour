using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PizzaParlour.CustomerManager.API.Repositories;
using Microsoft.Azure.Cosmos;

namespace PizzaParlour.CustomerManager.API.Functions
{
    public class GetCustomerById
    {
        private readonly ILogger<GetCustomerById> _logger;
        private readonly ICustomerAggregateRepository _customerAggregateRepository;

        public GetCustomerById(
            ILogger<GetCustomerById> logger,
            ICustomerAggregateRepository customerAggregateRepository)
        {
            _logger = logger;
            _customerAggregateRepository = customerAggregateRepository;
        }

        [FunctionName(nameof(GetCustomerById))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customer/{id}")] HttpRequest req,
            string id)
        {
            IActionResult result = null;

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogError($"Customer Id is not specified");
                    result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                }
                else
                {
                    var customer = await _customerAggregateRepository.GetCustomerById(id);
                    result = new OkObjectResult(customer);
                }              
            }
            catch (CosmosException cx) when (cx.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogError($"Customer Id: {id} was not found!");
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
