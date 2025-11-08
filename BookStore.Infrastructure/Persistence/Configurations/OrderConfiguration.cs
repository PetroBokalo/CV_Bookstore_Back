using BookStore.Domain.Entities;
using BookStore.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace BookStore.Infrastructure.Persistence.Configurations
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            // --- Primary key ---
            builder.HasKey(o => o.Id);

            // --- Relations ---
            builder.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(o => o.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Properties ---
            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(o => o.CreatedAt)
                  .IsRequired();
        }
    }

    internal class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            // --- Primary key ---
            builder.HasKey(i => i.Id);

            // --- Relations ---
            builder.HasOne(i => i.Order)
                   .WithMany(o => o.Items)
                   .HasForeignKey(i => i.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.Book)
                   .WithMany()
                   .HasForeignKey(i => i.BookId)
                   .OnDelete(DeleteBehavior.Restrict);

            // --- Properties ---
            builder.Property(i => i.Quantity)
                   .IsRequired();

            builder.Property(i => i.Price)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");
        }
    }




}
