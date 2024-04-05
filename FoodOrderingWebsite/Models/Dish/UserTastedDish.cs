using System.ComponentModel.DataAnnotations;

namespace FoodOrderingWebsite.Models.Dish
{
    public class UserTestedDish
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DishId { get; set; }
    }
}
