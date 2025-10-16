using BookStoreAPI.Models;

namespace BookStoreAPI.Repositories.Interfaces
{
    public interface IVerifyTokenRepository
    {
        Task AddAsync(VerifyEmailToken token);
        Task<VerifyEmailToken?> GetByIdAsync(int userId);
        Task<bool> ExistsByIdAsync(int userId);
        Task SaveChangesAsync();

    }
}
