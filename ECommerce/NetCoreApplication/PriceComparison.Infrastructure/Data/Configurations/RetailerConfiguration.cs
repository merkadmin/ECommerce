using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceComparison.Core.Entities;

namespace PriceComparison.Infrastructure.Data.Configurations;

public class RetailerConfiguration : IEntityTypeConfiguration<Retailer>
{
    public void Configure(EntityTypeBuilder<Retailer> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.LogoUrl)
            .HasMaxLength(500);

        builder.Property(r => r.BaseUrl)
            .HasMaxLength(500);

        builder.Property(r => r.AverageRating)
            .HasPrecision(3, 2);

        builder.HasIndex(r => r.Name);
        builder.HasIndex(r => r.IsActive);
    }
}
