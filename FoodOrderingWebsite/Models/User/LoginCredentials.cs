using System;
using System.ComponentModel.DataAnnotations;


namespace FoodOrderingWebsite.Models.User
{
    public class LoginCredentials
    {
        [Key]
        [Required(ErrorMessage = "The email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [MinLength(1, ErrorMessage = "The email address must be at least 1 character.")]
        public string Email { get; set; }

        [MinLength(1, ErrorMessage = "The password must be at least 1 characters.")]
        public string Password { get; set; }
    }
}
