using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Persistence.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            // --- Table name ---
            builder.ToTable("Books");

            // --- Primary Key ---
            builder.HasKey(b => b.Id);

            // --- Properties ---
            builder.Property(b => b.Price)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(b => b.DiscountPrice)
                   .HasColumnType("decimal(18,2)");

            builder.Property(b => b.HasDiscount)
                   .IsRequired();

            builder.Property(b => b.Title)
                   .IsRequired();

            builder.Property(b => b.Author)
                   .IsRequired();

            builder.Property(b => b.Publisher)
                   .IsRequired();

            builder.Property(b => b.Translator)
                   .HasMaxLength(200);

            builder.Property(b => b.PublicationYear)
                   .IsRequired()
                   .HasDefaultValue(2000);

            builder.Property(b => b.PagesNumber)
                   .IsRequired();

            builder.Property(b => b.Cover)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(b => b.ISBN)
                   .IsRequired()
                   .HasMaxLength(13);

            builder.Property(b => b.BookType)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(b => b.HasIllustrations)
                   .IsRequired();

            builder.Property(b => b.SoldAmount)
                   .HasDefaultValue(0);

            builder.Property(b => b.AddedTime)
                   .IsRequired();

            builder.Property(b => b.Capacity)
                   .IsRequired();

            // --- Relations ---
            builder.HasOne(b => b.BookGenre)
                   .WithMany()
                   .HasForeignKey(b => b.BookGenreId)
                   .OnDelete(DeleteBehavior.Cascade);

            // --- Owned type (BookFormat) ---
            builder.OwnsOne(b => b.Format, format =>
            {
                format.Property(f => f.WidthMm)
                      .IsRequired();

                format.Property(f => f.HeightMm)
                      .IsRequired();

                format.Property(f => f.ThicknessMm)
                      .HasDefaultValue(0);
            });

            // --- Collection of BookImage ---
            builder.HasMany<BookImage>()
                   .WithOne(i => i.Book)
                   .HasForeignKey(i => i.BookId)
                   .OnDelete(DeleteBehavior.Cascade);

            // --- Ignore computed property ---
            builder.Ignore(b => b.DiscountPercantage);
        }
    }
}
