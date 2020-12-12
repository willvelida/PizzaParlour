using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaParlour.Core.Models
{
    public class OrderItem
    {
        public string LineItem { get; set; }
        public double LineCost { get; set; }
        public int LineQuantity { get; set; }
    }
}
