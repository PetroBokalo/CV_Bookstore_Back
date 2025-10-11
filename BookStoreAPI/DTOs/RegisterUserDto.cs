using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        [MaxLength(50)]
        public string UserFirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string UserLastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

    }
}
