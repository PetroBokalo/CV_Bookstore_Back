using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.DTOs.Authentication
{
    public record ProvidePhoneNumberDto
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
