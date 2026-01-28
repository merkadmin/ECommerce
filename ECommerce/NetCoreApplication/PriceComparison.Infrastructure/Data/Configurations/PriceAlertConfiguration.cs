using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceComparison.Core.Entities;

namespace PriceComparison.Infrastructure.Data.Configurations;

public class PriceAlertConfiguration : IEntityTypeConfiguration<PriceAlert>
{
    public void Configure(EntityTypeBuilder<PriceAlert> builder)
    {
        builder.HasKey(pa => pa.Id);

        builder.Property(pa => pa.TargetPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(pa => pa.User)
            .WithMany(u => u.PriceAlerts)
            .HasForeignKey(pa => pa.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pa => pa.Product)
            .WithMany(p => p.PriceAlerts)
            .HasForeignKey(pa => pa.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pa => pa.UserId);
        builder.HasIndex(pa => pa.ProductId);
        builder.HasIndex(pa => pa.IsActive);
        builder.HasIndex(pa => new { pa.UserId, pa.ProductId }).IsUnique();
    }
}
