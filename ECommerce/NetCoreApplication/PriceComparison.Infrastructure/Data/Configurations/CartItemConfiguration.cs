using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceComparison.Core.Entities;

namespace PriceComparison.Infrastructure.Data.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        builder.HasOne(c => c.User)
            .WithMany(u => u.CartItems)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Retailer)
            .WithMany()
            .HasForeignKey(c => c.RetailerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.ProductId);
        builder.HasIndex(c => new { c.UserId, c.ProductId, c.RetailerId }).IsUnique();
    }
}
