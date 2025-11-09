using System.ComponentModel.DataAnnotations;

namespace BookStore.Application.DTOs.Authentication
{
    public record GoogleLoginUserDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;

    }
}
