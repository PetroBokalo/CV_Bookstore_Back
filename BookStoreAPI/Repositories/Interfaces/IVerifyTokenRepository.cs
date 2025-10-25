using BookStoreAPI.Entities;

namespace BookStoreAPI.Repositories.Interfaces
{
    public interface IVerifyTokenRepository
    {
        Task AddAsync(VerifyEmailToken token);
        Task<VerifyEmailToken?> GetByUserIdAsync(int userId);
        Task<bool> ExistsByIdAsync(int userId);
        Task SaveChangesAsync();

    }
}
