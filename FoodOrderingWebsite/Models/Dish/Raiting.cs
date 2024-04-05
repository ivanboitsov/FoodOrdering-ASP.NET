using System.ComponentModel.DataAnnotations;

namespace FoodOrderingWebsite.Models.Dish
{
    public class Raiting
    {
        [Key]
        public Guid Id { get; set; }
        public Guid DishId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
    }
}
