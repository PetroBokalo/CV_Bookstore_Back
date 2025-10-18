using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.Entities
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
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        

        [Required]
        public Role Role { get; set; } = Role.User;

        [Required]
        public string? UserFirstName { get; set; } = string.Empty;

        [Required]
        public string? UserLastName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }

        public DateTime? EmailConfirmedAt { get; set; }

        public bool IsEmailConfirmed => EmailConfirmedAt.HasValue; // possibly useless

      
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
