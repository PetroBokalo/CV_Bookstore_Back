using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.DTOs
{
    public class ProvidePhoneNumberDto
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
