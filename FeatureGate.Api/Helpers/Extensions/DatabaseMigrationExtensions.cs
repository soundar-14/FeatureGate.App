using FeatureGate.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FeatureGate.Api.Helpers.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class DatabaseMigrationExtensions
    {
        public static async Task MigrateDatabaseAsync(this IHost app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FeatureGateDbContext>();

            await dbContext.Database.MigrateAsync();
        }
    }
}
