using System.ComponentModel.DataAnnotations;

namespace FoodOrderingWebsite.Models.Address
{
    public class AddressResponseModel
    {
        [Key]
        public long Id { get; set; }
        public Guid objectGuid { get; set; }
        public string text { get; set; }
        public string objectLevel { get; set; }
        public string objectLevelText { get; set; }
    }
}
