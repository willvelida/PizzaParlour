using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PizzaParlour.CustomerManager.API.Repositories;
using Microsoft.Azure.Cosmos;

namespace PizzaParlour.CustomerManager.API.Functions
{
    public class DeleteCustomer
    {
        private readonly ILogger<DeleteCustomer> _logger;
        private readonly ICustomerAggregateRepository _customerAggregateRepository;

        public DeleteCustomer(
            ILogger<DeleteCustomer> logger,
            ICustomerAggregateRepository customerAggregateRepository)
        {
            _logger = logger;
            _customerAggregateRepository = customerAggregateRepository;
        }

        [FunctionName(nameof(DeleteCustomer))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Customer/{id}")] HttpRequest req,
            string id)
        {
            IActionResult result = null;

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogError($"Customer Id is not supplied");
                    result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                }

                await _customerAggregateRepository.DeleteCustomer(id);
                result = new StatusCodeResult(StatusCodes.Status200OK);
            }
            catch (CosmosException cex) when (cex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
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
