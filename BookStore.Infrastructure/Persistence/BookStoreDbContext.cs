using BookStore.Domain.Entities;
using BookStore.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BookStore.Infrastructure.Persistence
{
    public class BookStoreDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        protected readonly IConfiguration configuration;

        public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options, IConfiguration configuration)
            : base (options)
        {
            this.configuration = configuration;
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasOne(u => u.Cart)
                .WithOne()
                .HasForeignKey<Cart>(c => c.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ApplyConfigurationsFromAssembly(typeof(BookStoreDbContext).Assembly);
        }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<BookGenre> BookGenres { get; set; }

        public DbSet<VerifyEmailToken> VerifyEmailTokens { get; set; }

       
    }
}
