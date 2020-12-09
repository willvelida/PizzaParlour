using PizzaParlour.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PizzaParlour.CustomerManager.Aggregate.Repositories
{
    public interface ICustomerAggregateRepository
    {
        /// <summary>
        /// Inserts or Upserts a customer into the CustomerAggregate Container in Cosmos DB.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        Task UpsertCustomer(Customer customer);
    }
}
