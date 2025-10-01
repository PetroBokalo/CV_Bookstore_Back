using BookStoreAPI.Models;

namespace BookStoreAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task<bool> ExistsByEmail(string email);
        Task SaveChangesAsync();


    }
}
 