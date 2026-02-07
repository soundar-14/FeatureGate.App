using FeatureGate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace FeatureGate.Infrastructure.Configurations
{
    [ExcludeFromCodeCoverage]
    public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
    {
        public void Configure(EntityTypeBuilder<Feature> builder)
        {
            builder.ToTable("features");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Key)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.DefaultState)
                .IsRequired();

            builder.Property(f => f.Description)
                .HasMaxLength(500);

            builder.HasIndex(f => f.Key)
                .IsUnique();

            builder.HasMany(f => f.Overrides)
                .WithOne(o => o.Feature)
                .HasForeignKey(o => o.FeatureId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
