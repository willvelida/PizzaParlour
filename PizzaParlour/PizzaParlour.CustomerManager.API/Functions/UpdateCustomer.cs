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
using PizzaParlour.Core.Models;
using Microsoft.Azure.Cosmos;

namespace PizzaParlour.CustomerManager.API.Functions
{
    public class UpdateCustomer
    {
        private readonly ILogger<UpdateCustomer> _logger;
        private readonly ICustomerAggregateRepository _customerAggregateRepository;
        private readonly ICustomerRepository _customerRepository;

        public UpdateCustomer(
            ILogger<UpdateCustomer> logger,
            ICustomerAggregateRepository customerAggregateRepository,
            ICustomerRepository customerRepository)
        {
            _logger = logger;
            _customerAggregateRepository = customerAggregateRepository;
            _customerRepository = customerRepository;
        }

        [FunctionName(nameof(UpdateCustomer))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Customer/{id}")] HttpRequest req,
            string id)
        {
            IActionResult result = null;

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogError($"Customer Id has not been specified");
                    result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                }
                else
                {
                    var oldCustomer = await _customerAggregateRepository.GetCustomerById(id);

                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                    var newCustomer = JsonConvert.DeserializeObject<Customer>(requestBody);

                    newCustomer.CustomerId = oldCustomer.CustomerId;

                    await _customerRepository.AddCustomer(newCustomer);

                    result = new StatusCodeResult(StatusCodes.Status200OK);
                }                                
            }
            catch (CosmosException cex) when (cex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogError($"Could not find Customer with ID: {id}");
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
