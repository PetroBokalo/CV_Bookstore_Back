using BookStoreAPI.Data;
using BookStoreAPI.Entities;
using BookStoreAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Repositories.Implementations
{
    public class VerifyTokenRepository : IVerifyTokenRepository
    {
        private readonly BookStoreDbContext _dbContext;

        public VerifyTokenRepository(BookStoreDbContext context)
        {
            this._dbContext = context;
        }

        public async Task AddAsync(VerifyEmailToken token) =>
            await _dbContext.VerifyEmailTokens.AddAsync(token);

        public async Task<bool> ExistsByIdAsync(int userId) =>
            await _dbContext.VerifyEmailTokens.AnyAsync(token => token.AppUserId == userId);


        public async Task<VerifyEmailToken?> GetByUserIdAsync(int userId) =>
            await _dbContext.VerifyEmailTokens.FirstOrDefaultAsync(token => token.AppUserId == userId);


        public async Task SaveChangesAsync() =>
            await _dbContext.SaveChangesAsync();



    }
}
