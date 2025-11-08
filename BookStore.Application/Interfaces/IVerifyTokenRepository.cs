using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces
{
    public interface IVerifyTokenRepository
    {
        Task AddAsync(VerifyEmailToken token);
        Task<VerifyEmailToken?> GetByUserIdAsync(int userId);
        Task<bool> ExistsByIdAsync(int userId);
        Task SaveChangesAsync();

    }
}
