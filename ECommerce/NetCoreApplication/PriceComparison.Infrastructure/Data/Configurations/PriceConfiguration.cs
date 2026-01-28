using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceComparison.Core.Entities;

namespace PriceComparison.Infrastructure.Data.Configurations;

public class PriceConfiguration : IEntityTypeConfiguration<Price>
{
    public void Configure(EntityTypeBuilder<Price> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.CurrentPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.OriginalPrice)
            .HasPrecision(18, 2);

        builder.Property(p => p.DiscountPercent)
            .HasPrecision(5, 2);

        builder.Property(p => p.ShippingCost)
            .HasPrecision(18, 2);

        builder.Property(p => p.ProductUrl)
            .HasMaxLength(1000);

        builder.HasOne(p => p.Product)
            .WithMany(pr => pr.Prices)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Retailer)
            .WithMany(r => r.Prices)
            .HasForeignKey(p => p.RetailerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.ProductId);
        builder.HasIndex(p => p.RetailerId);
        builder.HasIndex(p => new { p.ProductId, p.RetailerId }).IsUnique();
        builder.HasIndex(p => p.CurrentPrice);
        builder.HasIndex(p => p.IsAvailable);
    }
}
