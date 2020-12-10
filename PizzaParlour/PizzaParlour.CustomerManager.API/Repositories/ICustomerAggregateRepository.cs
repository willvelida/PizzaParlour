using PizzaParlour.Core.Models;
using System.Threading.Tasks;

namespace PizzaParlour.CustomerManager.API.Repositories
{
    /// <summary>
    /// Interfact defining contract for CustomerAggregateRepository
    /// </summary>
    public interface ICustomerAggregateRepository
    {
        /// <summary>
        /// Retrieves a customer from the CustomerAggregate collection based on its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Customer> GetCustomerById(string id);

        /// <summary>
        /// Deletes a customer based on its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteCustomer(string id);
    }
}
