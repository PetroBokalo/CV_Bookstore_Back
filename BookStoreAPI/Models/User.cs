using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.Models
{
    public class User
    {

        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

        [Required]
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        [Required]
        public Role Role { get; set; } = Role.User;

        public string? UserName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }

        public DateTime? EmailConfirmedAt { get; set; }

        public bool IsEmailConfirmed => EmailConfirmedAt.HasValue;

      
        public bool IsBanned { get; set; } = false;

        [MaxLength(200)]
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }  

    }

    public enum Role
    {
        User,
        Admin
    }

}
