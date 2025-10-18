using BookStoreAPI.Data;
using BookStoreAPI.Entities;
using BookStoreAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Repositories.Implementations
{
    public class ResetPasswordTokenRepository : IResetPasswordTokenRepository
    {
        private readonly BookStoreDbContext _dbContext;

        public ResetPasswordTokenRepository(BookStoreDbContext context)
        {
            this._dbContext = context;
        }

        public async Task AddAsync(ResetPasswordToken token) =>
            await _dbContext.ResetPasswordTokens.AddAsync(token);

        public async Task<bool> ExistsByIdAsync(int userId) =>
            await _dbContext.ResetPasswordTokens.AnyAsync(token => token.UserId == userId);


        public async Task<ResetPasswordToken?> GetByIdAsync(int userId) =>
            await _dbContext.ResetPasswordTokens.FirstOrDefaultAsync(token => token.UserId == userId);


        public async Task SaveChangesAsync() =>
            await _dbContext.SaveChangesAsync();


    }
}
