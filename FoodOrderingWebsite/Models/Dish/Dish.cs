using System.ComponentModel.DataAnnotations;

namespace FoodOrderingWebsite.Models.Dish
{
    public class Dish
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public bool Vegetarian { get; set; }

        public double Rating { get; set; }

        public string Description { get; set; }
        public string Image { get; set; }
    }
}
