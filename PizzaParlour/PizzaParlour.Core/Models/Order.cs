using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaParlour.Core.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; }
        public DeliveryMethodType DeliveryMethod { get; set; }
        public string DeliveryInstructions { get; set; }
        public Address OrderAddress { get; set; }
        public double OrderCost { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public string OrderStatus { get; set; }
    }

    public enum DeliveryMethodType
    {
        Pickup,
        Delivery
    }

    public enum PaymentMethodType
    {
        Cash,
        CreditCard
    }
}
