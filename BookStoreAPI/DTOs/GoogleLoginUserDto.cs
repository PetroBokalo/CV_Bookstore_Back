using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.DTOs
{
    public class GoogleLoginUserDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;

    }
}
