using System.ComponentModel.DataAnnotations;

namespace FoodOrderingWebsite.Models.Order
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime DeliveryTime { get; set; }
        public DateTime OrderTime { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public Guid Address { get; set; }
    }
}
