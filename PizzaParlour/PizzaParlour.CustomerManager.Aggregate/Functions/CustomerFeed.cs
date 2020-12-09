using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PizzaParlour.Core.Models;
using PizzaParlour.CustomerManager.Aggregate.Repositories;

namespace PizzaParlour.CustomerManager.Aggregate.Functions
{
    public class CustomerFeed
    {
        private readonly ILogger<CustomerFeed> _logger;
        private readonly ICustomerAggregateRepository _customerAggregateRepository;

        public CustomerFeed(
            ILogger<CustomerFeed> logger,
            ICustomerAggregateRepository customerAggregateRepository)
        {
            _logger = logger;
            _customerAggregateRepository = customerAggregateRepository;
        }

        [FunctionName(nameof(CustomerFeed))]
        public async Task Run([CosmosDBTrigger(
            databaseName: "PizzaParlourDB",
            collectionName: "Customers",
            ConnectionStringSetting = "CosmosDBConnectionString",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionPrefix = "Customers")]IReadOnlyList<Document> input, ILogger log)
        {
            try
            {
                if (input != null && input.Count > 0)
                {
                    foreach (var document in input)
                    {
                        var customer = JsonConvert.DeserializeObject<Customer>(document.ToString());

                        await _customerAggregateRepository.UpsertCustomer(customer);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown: {ex.Message}");
                throw;
            }

            
        }
    }
}
