namespace FoodOrderingWebsite.Models.Basket
{
    public class Basket
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
        public Guid DishId { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string Image { get; set; }
    }
}
