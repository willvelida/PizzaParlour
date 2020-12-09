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
using PizzaParlour.CustomerManager.API.Repositories;
using PizzaParlour.Core.Models;

namespace PizzaParlour.CustomerManager.API.Functions
{
    public class CreateCustomer
    {
        private readonly ILogger<CreateCustomer> _logger;
        private readonly ICustomerRepository _customerRepository;

        public CreateCustomer(
            ILogger<CreateCustomer> logger,
            ICustomerRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        [FunctionName(nameof(CreateCustomer))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customer")] HttpRequest req)
        {
            IActionResult result = null;

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var customer = JsonConvert.DeserializeObject<Customer>(requestBody);

                await _customerRepository.AddCustomer(customer);

                result = new StatusCodeResult(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                // TODO: Send this exception to Service Bus or App Insights
                _logger.LogError($"Internal Server Error. Exception thrown: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
