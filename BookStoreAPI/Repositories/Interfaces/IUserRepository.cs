using BookStoreAPI.Entities;

namespace BookStoreAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task AddAsync(User user);
        Task<bool> ExistsByEmail(string email);
        Task<bool> ExistsByIdAsync(int id);       
        Task SaveChangesAsync();


    }
}
 