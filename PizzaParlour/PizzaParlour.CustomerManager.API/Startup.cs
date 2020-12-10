using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PizzaParlour.CustomerManager.API;
using PizzaParlour.CustomerManager.API.Repositories;
using System.IO;

[assembly: FunctionsStartup(typeof(Startup))]
namespace PizzaParlour.CustomerManager.API
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);

            var cosmosClientOptions = new CosmosClientOptions()
            {
                ConnectionMode = ConnectionMode.Direct
            };

            builder.Services.AddSingleton((s) => new CosmosClient(config["CosmosDBConnectionString"], cosmosClientOptions));
            builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
            builder.Services.AddSingleton<ICustomerAggregateRepository, CustomerAggregateRepository>();
        }
    }
}
