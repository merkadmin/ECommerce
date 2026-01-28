using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceComparison.Core.Entities;

namespace PriceComparison.Infrastructure.Data.Configurations;

public class PriceHistoryConfiguration : IEntityTypeConfiguration<PriceHistory>
{
    public void Configure(EntityTypeBuilder<PriceHistory> builder)
    {
        builder.HasKey(ph => ph.Id);

        builder.Property(ph => ph.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(ph => ph.Product)
            .WithMany(p => p.PriceHistories)
            .HasForeignKey(ph => ph.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ph => ph.Retailer)
            .WithMany(r => r.PriceHistories)
            .HasForeignKey(ph => ph.RetailerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ph => ph.ProductId);
        builder.HasIndex(ph => ph.RetailerId);
        builder.HasIndex(ph => ph.RecordedAt);
        builder.HasIndex(ph => new { ph.ProductId, ph.RetailerId, ph.RecordedAt });
    }
}
