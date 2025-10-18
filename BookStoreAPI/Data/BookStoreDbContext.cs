using BookStoreAPI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Data
{
    public class BookStoreDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        protected readonly IConfiguration configuration;

        public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options, IConfiguration configuration)
            : base (options)
        {
            this.configuration = configuration;
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //{
        //    options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"));
        //}


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.AppUser)
                .HasForeignKey<Cart>(c => c.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

        }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<BookGenre> BookGenres { get; set; }

        public DbSet<VerifyEmailToken> VerifyEmailTokens { get; set; }

        public DbSet<ResetPasswordToken> ResetPasswordTokens { get; set; }

       
    }
}
