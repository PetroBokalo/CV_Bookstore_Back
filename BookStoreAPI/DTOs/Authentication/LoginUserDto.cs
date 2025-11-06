using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.DTOs.Authentication
{
    public record LoginUserDto
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = true;
      
    }
}
