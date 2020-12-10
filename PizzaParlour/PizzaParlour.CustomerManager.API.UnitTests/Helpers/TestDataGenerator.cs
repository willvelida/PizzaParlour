using PizzaParlour.Core.Models;
using System;

namespace PizzaParlour.CustomerManager.API.UnitTests.Helpers
{
    public static class TestDataGenerator
    {
        public static Customer GenerateCustomer()
        {
            return new Customer()
            {
                CustomerId = Guid.NewGuid().ToString(),
                Name = "Will Velida",
                Email = "will@test.com",
                Password = "SomePassword",
                PhoneNumber = "0800111222",
                Address = new CustomerAddress()
                {
                    AddressLine1 = "Address Line 1",
                    AddressLine2 = "Address Line 2",
                    City = "City",
                    State = "State",
                    ZipCode = 1000
                }
            };
        }
    }
}
