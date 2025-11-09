using System.ComponentModel.DataAnnotations;

namespace BookStore.Application.DTOs.Authentication
{
    public record ProvidePhoneNumberDto
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
