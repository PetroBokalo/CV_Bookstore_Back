using System.ComponentModel.DataAnnotations;

namespace BookStore.Application.DTOs.Authentication
{
    public record ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string UserURI { get; set; } = string.Empty;

    }
}
