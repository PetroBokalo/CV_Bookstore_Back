
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Configurations
{
    internal class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts");

            // --- Primary key ---
            builder.HasKey(c => c.Id);

            // --- Relations ---
            builder.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(o => o.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Items collection ---
            builder.HasMany(c => c.Items)
                   .WithOne(i => i.Cart)
                   .HasForeignKey(i => i.CartId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
