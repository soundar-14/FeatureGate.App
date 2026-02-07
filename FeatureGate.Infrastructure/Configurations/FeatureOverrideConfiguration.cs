
using FeatureGate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace FeatureGate.Infrastructure.Configurations
{
    [ExcludeFromCodeCoverage]
    public class FeatureOverrideConfiguration : IEntityTypeConfiguration<FeatureOverride>
    {
        public void Configure(EntityTypeBuilder<FeatureOverride> builder)
        {
            builder.ToTable("feature_overrides");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.TargetType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(o => o.TargetId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.State)
                .IsRequired();

            builder.HasIndex(o => new
            {
                o.FeatureId,
                o.TargetType,
                o.TargetId
            })
            .IsUnique();
        }
    }
}
