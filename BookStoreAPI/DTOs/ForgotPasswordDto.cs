using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.DTOs
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
