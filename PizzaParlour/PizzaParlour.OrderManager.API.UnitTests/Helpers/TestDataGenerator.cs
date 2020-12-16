using PizzaParlour.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PizzaParlour.OrderManager.API.UnitTests.Helpers
{
    public static class TestDataGenerator
    {
        public static Order GenerateOrder()
        {
            var items = new List<OrderItem>()
            {
                new OrderItem
                {
                    LineItem = "Pineapple",
                    LineQuantity = 1,
                    LineCost = 12.99
                }
            };

            var order = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                CustomerId = Guid.NewGuid().ToString(),
                OrderDate = DateTime.Now,
                Items = items,
                DeliveryMethod = DeliveryMethodType.Delivery,
                DeliveryInstructions = "",
                OrderAddress = new Address
                {
                    AddressLine1 = "Address Line 1",
                    AddressLine2 = "Address Line 2",
                    City = "Auckland",
                    State = "Auckland",
                    ZipCode = 1099
                },
                OrderCost = items.First().LineCost,
                PaymentMethod = PaymentMethodType.CreditCard,
                OrderStatus = "Ordered"
            };

            return order;
        }
    }
}
