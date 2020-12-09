using PizzaParlour.Core.Models;
using System.Threading.Tasks;

namespace PizzaParlour.CustomerManager.API.Repositories
{
    /// <summary>
    /// Interface defining contracts to be used in the Customer Repository class
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Adds a customer to the Customer collection in Cosmos DB
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        Task AddCustomer(Customer customer);
    }
}
