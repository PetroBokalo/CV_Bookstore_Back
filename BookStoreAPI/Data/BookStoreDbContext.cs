using BookStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Data
{
    public class BookStoreDbContext : DbContext
    {
        protected readonly IConfiguration configuration;

        public BookStoreDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"));
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<BookGenre> BookGenres { get; set; }

        public DbSet<VerifyEmailToken> VerifyEmailTokens { get; set; }

    }
}
