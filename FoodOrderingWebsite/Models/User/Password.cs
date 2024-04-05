using System.ComponentModel.DataAnnotations;

namespace FoodOrderingWebsite.Models.User
{
    public class Password
    {
        [Key]
        public Guid UserId { get; set; }
        public string PasswordValue { get; set; }
    }
}
