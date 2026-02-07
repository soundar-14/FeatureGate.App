using FeatureGate.Domain.Entities;
using FeatureGate.Infrastructure.Contexts;
using FeatureGate.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FeatureGate.Infrastructure.Tests.Repositories;

public class FeatureRepositoryTests
{
    private FeatureGateDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<FeatureGateDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new FeatureGateDbContext(options);
    }

    [Fact]
    public async Task AddAsync_SaveChangesAsync_GetByIdAsync_ShouldPersistFeature()
    {
        using var context = CreateDbContext();
        var repo = new FeatureRepository(context);

        var feature = new Feature
        {
            Id = Guid.NewGuid(),
            Key = "feature-a",          // stored lowercase
            DefaultState = true,
            Description = "Test feature"
        };

        // Act
        await repo.AddAsync(feature);
        await repo.SaveChangesAsync();

        var result = await repo.GetByIdAsync(feature.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Key.Should().Be("feature-a");
        result.DefaultState.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllFeatures()
    {
        using var context = CreateDbContext();
        var repo = new FeatureRepository(context);

        context.Features.AddRange(
            new Feature { Id = Guid.NewGuid(), Key = "f1", DefaultState = true },
            new Feature { Id = Guid.NewGuid(), Key = "f2", DefaultState = false }
        );

        await context.SaveChangesAsync();

        // Act
        var result = await repo.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Update_ShouldModifyFeature()
    {
        using var context = CreateDbContext();
        var repo = new FeatureRepository(context);

        var feature = new Feature
        {
            Id = Guid.NewGuid(),
            Key = "feature-update",
            DefaultState = false
        };

        context.Features.Add(feature);
        await context.SaveChangesAsync();

        // Act
        feature.DefaultState = true;
        repo.Update(feature);
        await repo.SaveChangesAsync();

        // Assert
        var updated = await context.Features.FindAsync(feature.Id);
        updated!.DefaultState.Should().BeTrue();
    }

    [Fact]
    public async Task Remove_ShouldDeleteFeature()
    {
        using var context = CreateDbContext();
        var repo = new FeatureRepository(context);

        var feature = new Feature
        {
            Id = Guid.NewGuid(),
            Key = "feature-remove",
            DefaultState = true
        };

        context.Features.Add(feature);
        await context.SaveChangesAsync();

        // Act
        repo.Remove(feature);
        await repo.SaveChangesAsync();

        // Assert
        var deleted = await context.Features.FindAsync(feature.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task GetByKeyAsync_ShouldReturnFeature_WhenKeyExists()
    {
        using var context = CreateDbContext();
        var repo = new FeatureRepository(context);

        context.Features.Add(new Feature
        {
            Id = Guid.NewGuid(),
            Key = "new-dashboard",     // lowercase
            DefaultState = true
        });

        await context.SaveChangesAsync();

        // Act
        var result = await repo.GetByKeyAsync("new-dashboard");

        // Assert
        result.Should().NotBeNull();
        result!.Key.Should().Be("new-dashboard");
    }

    [Fact]
    public async Task GetByKeyAsync_ShouldReturnNull_WhenFeatureDoesNotExist()
    {
        using var context = CreateDbContext();
        var repo = new FeatureRepository(context);

        // Act
        var result = await repo.GetByKeyAsync("missing-feature");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByKeyAsync_ShouldBeCaseSensitive_IfKeyNotNormalized()
    {
        using var context = CreateDbContext();
        var repo = new FeatureRepository(context);

        context.Features.Add(new Feature
        {
            Id = Guid.NewGuid(),
            Key = "beta-feature",   // stored lowercase
            DefaultState = true
        });

        await context.SaveChangesAsync();

        // Act
        var result = await repo.GetByKeyAsync("BETA-FEATURE");

        // Assert
        result.Should().BeNull();
    }
}
