using BookStoreAPI.Entities;

namespace BookStoreAPI.Repositories.Interfaces
{
    public interface IResetPasswordTokenRepository
    {
        Task AddAsync(ResetPasswordToken token);
        Task<bool> ExistsByIdAsync(int userId);
        Task<ResetPasswordToken?> GetByIdAsync(int userId);
        Task SaveChangesAsync();
    }
}