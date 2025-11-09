
namespace BookStore.Domain.Entities
{
    public class VerifyEmailToken
    {

        public int Id { get; set; }

        public int AppUserId { get; set; }

        public string Code { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsUsed { get; set; }

        public int Attemps { get; set; } 

    }
}
 