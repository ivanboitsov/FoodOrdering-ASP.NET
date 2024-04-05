using FoodOrderingWebsite.Models.Basket;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderingWebsite.Models.Order
{
    public class OrderResponseModel
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime DeliveryTime { get; set; }
        public DateTime OrderTime { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public List<BasketResponseModel> dishes { get; set; }
        public Guid Address { get; set; }
    }
}
