using BookStoreAPI.Data;
using BookStoreAPI.Entities;
using BookStoreAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {

        private readonly BookStoreDbContext _dbContext;

        public UserRepository(BookStoreDbContext context)
        {
            this._dbContext = context;
        }


        public async Task AddAsync(User user) =>       
            await _dbContext.AddAsync(user);

        public async Task<bool> ExistsByEmail(string email) =>
            await _dbContext.Users.AnyAsync(u => u.Email == email);

        public async Task<bool> ExistsByIdAsync(int id) =>
            await _dbContext.Users.AnyAsync(u => u.Id == id);


        public Task<User?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public  Task<User?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public  Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }

        //public async Task<User?> GetByEmailAsync(string email) =>
        //     await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

        //public async Task<User?> GetByIdAsync(int id) =>
        //    await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        //public async Task<User?> GetByRefreshTokenAsync(string refreshToken) =>
        //    await _dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);


        public async Task SaveChangesAsync() =>
            await _dbContext.SaveChangesAsync();
       
    }
}
