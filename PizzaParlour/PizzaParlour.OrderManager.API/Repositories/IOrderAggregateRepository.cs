using PizzaParlour.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaParlour.OrderManager.API.Repositories
{
    public interface IOrderAggregateRepository
    {
        /// <summary>
        /// Retrieves a single order for a customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        Task<Order> GetAnOrderByCustomerId(string customerId, string orderId);

        /// <summary>
        /// Retrieves all the orders for a customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        Task<List<Order>> GetAllOrdersByCustomerId(string customerId);

        /// <summary>
        /// Deletes an order for a customer from the OrderAggregate collection
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task DeleteOrder(string customerId, string orderId);
    }
}
