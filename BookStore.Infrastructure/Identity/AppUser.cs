
using BookStore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Infrastructure.Identity
{
    public class AppUser : IdentityUser<int>
    {
        [Required]
        public string UserFirstName { get; set; } = string.Empty;

        [Required]
        public string UserLastName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }

        public bool IsBanned { get; set; } = false;

        [MaxLength(200)]
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        public Cart? Cart { get; set; } = null!;

    }
}
