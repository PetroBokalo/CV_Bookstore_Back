using BookStore.Infrastructure.Persistence;
using BookStore.Domain.Entities;
using BookStore.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories
{
    public class VerifyTokenRepository : IVerifyTokenRepository
    {
        private readonly BookStoreDbContext _dbContext;

        public VerifyTokenRepository(BookStoreDbContext context)
        {
            _dbContext = context;
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
