namespace FoodOrderingWebsite.Models.Basket
{
    public class BasketResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string Image { get; set; }
    }
}
