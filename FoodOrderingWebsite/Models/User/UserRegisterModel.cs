using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FoodOrderingWebsite.Models.User
{
    public class UserRegisterModel
    {
        [Key]
        [Required(ErrorMessage = "The full name is required.")]
        [MinLength(1, ErrorMessage = "The full name must be at least 1 character.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "The password is required.")]
        [MinLength(6, ErrorMessage = "The password must be at least 6 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "The email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [MinLength(1, ErrorMessage = "The email address must be at least 1 character.")]
        public string Email { get; set; }

        public Guid? AddressId { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "The 'gender' field is required.")]
        [EnumDataType(typeof(Gender), ErrorMessage = "The 'gender' must be 'Male' or 'Female'.")]
        public Gender Gender { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

    }
    public enum Gender
    {
        Male,
        Female
    }
}
