using FeatureGate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FeatureGate.Infrastructure.Contexts
{
    [ExcludeFromCodeCoverage]
    public class FeatureGateDbContext : DbContext
    {
        public FeatureGateDbContext(DbContextOptions<FeatureGateDbContext> options)
            : base(options)
        {
        }

        public DbSet<Feature> Features => Set<Feature>();
        public DbSet<FeatureOverride> FeatureOverrides => Set<FeatureOverride>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(FeatureGateDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
