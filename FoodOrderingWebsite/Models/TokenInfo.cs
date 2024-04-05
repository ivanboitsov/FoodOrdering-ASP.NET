using System.ComponentModel.DataAnnotations;

namespace FoodOrderingWebsite.Models
{
    public class TokenInfo
    {
        [Key]
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsValid { get; set; }
    }
}
