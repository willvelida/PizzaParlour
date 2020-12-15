using PizzaParlour.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PizzaParlour.OrderManager.API.Repositories
{
    public interface IOrderRepository
    {
        /// <summary>
        /// Creates a new Order in the Order collection in Cosmos DB
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task AddOrder(Order order);
    }
}
