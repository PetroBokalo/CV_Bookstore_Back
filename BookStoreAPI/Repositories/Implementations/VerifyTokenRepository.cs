using BookStoreAPI.Data;
using BookStoreAPI.Models;
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
            await _dbContext.VerifyEmailTokens.AnyAsync(token => token.UserId == userId);
        

        public async Task<VerifyEmailToken?> GetByIdAsync(int userId) =>
            await _dbContext.VerifyEmailTokens.FirstOrDefaultAsync(token => token.UserId == userId);


        public async Task SaveChangesAsync() =>
            await _dbContext.SaveChangesAsync();



    }
}
