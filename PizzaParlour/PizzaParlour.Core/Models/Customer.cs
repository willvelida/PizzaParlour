using Newtonsoft.Json;

namespace PizzaParlour.Core.Models
{
    public class Customer
    {
        [JsonProperty("id")]
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public CustomerAddress Address { get; set; }

    }
}
