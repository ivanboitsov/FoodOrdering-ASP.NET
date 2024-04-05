using System.ComponentModel.DataAnnotations;

namespace FoodOrderingWebsite.Models.User
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public Guid Address { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
