

using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Configurations
{
    internal class BookGenreConfiguration : IEntityTypeConfiguration<BookGenre>
    {
        public void Configure(EntityTypeBuilder<BookGenre> builder)
        {
            builder.ToTable("BookGenres");

            // --- Primary key ---
            builder.HasKey(g => g.Id);

            // --- Properties ---
            builder.Property(g => g.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(g => g.Description)
                   .HasMaxLength(500);

            // --- Relations ---
            builder.HasMany(g => g.Books)
                .WithMany(b => b.Genres)
                .UsingEntity<Dictionary<string, object>>(
                "BookBookGenre",
                j => j.HasOne<Book>()
                             .WithMany()
                             .HasForeignKey("BookId")
                             .OnDelete(DeleteBehavior.Cascade),
                       j => j.HasOne<BookGenre>()
                             .WithMany()
                             .HasForeignKey("BookGenreId")
                             .OnDelete(DeleteBehavior.Cascade),
                       j =>
                       {
                           j.HasKey("BookId", "BookGenreId");
                           j.ToTable("BookBookGenres");
                       });



        }
    }
}
