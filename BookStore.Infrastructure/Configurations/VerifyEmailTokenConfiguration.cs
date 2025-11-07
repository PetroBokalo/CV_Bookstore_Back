using BookStore.Domain.Entities;
using BookStore.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Configurations
{
    internal class VerifyEmailTokenConfiguration : IEntityTypeConfiguration<VerifyEmailToken>
    {
        public void Configure(EntityTypeBuilder<VerifyEmailToken> builder)
        {
            builder.ToTable("VerifyEmailTokens");

            // --- Primary key ---
            builder.HasKey(t => t.Id);

            // --- Relations ---
            builder.HasOne<AppUser>()
                   .WithMany()
                   .HasForeignKey(t => t.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // --- Properties ---
            builder.Property(t => t.Code)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(t => t.ExpiresAt)
                   .IsRequired();

            builder.Property(t => t.CreatedAt)
                   .IsRequired();

            builder.Property(t => t.IsUsed)
                   .IsRequired();

            builder.Property(t => t.Attemps)
                   .IsRequired();
        }
    }
}
