﻿using BookStoreAPI.Entities;

namespace BookStoreAPI.Entities
{
    public class ResetPasswordToken
    {

        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsUsed { get; set; }
        

    }
}
