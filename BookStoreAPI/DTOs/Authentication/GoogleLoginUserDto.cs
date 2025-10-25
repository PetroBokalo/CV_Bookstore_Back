using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.DTOs.Authentication
{
    public record GoogleLoginUserDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;

    }
}
