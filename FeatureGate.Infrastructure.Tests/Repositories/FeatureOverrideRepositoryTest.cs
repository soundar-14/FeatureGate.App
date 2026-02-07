
using FeatureGate.Domain.Entities;
using FeatureGate.Domain.Enums;
using FeatureGate.Infrastructure.Contexts;
using FeatureGate.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FeatureGate.Infrastructure.Tests.Repositories;

public class FeatureOverrideRepositoryTests
{
    private FeatureGateDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<FeatureGateDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new FeatureGateDbContext(options);
    }

    [Fact]
    public async Task AddAsync_SaveChangesAsync_GetByIdAsync_ShouldPersistOverride()
    {
        using var context = CreateDbContext();
        var repo = new FeatureOverrideRepository(context);

        var overrideEntity = new FeatureOverride
        {
            Id = Guid.NewGuid(),
            FeatureId = Guid.NewGuid(),
            TargetType = OverrideType.User,
            TargetId = "user-1",
            State = true
        };

        // Act
        await repo.AddAsync(overrideEntity);
        await repo.SaveChangesAsync();

        var result = await repo.GetByIdAsync(overrideEntity.Id);

        // Assert
        result.Should().NotBeNull();
        result!.TargetId.Should().Be("user-1");
        result.State.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllOverrides()
    {
        using var context = CreateDbContext();
        var repo = new FeatureOverrideRepository(context);

        context.FeatureOverrides.AddRange(
            new FeatureOverride
            {
                Id = Guid.NewGuid(),
                FeatureId = Guid.NewGuid(),
                TargetType = OverrideType.User,
                TargetId = "user-1",
                State = true
            },
            new FeatureOverride
            {
                Id = Guid.NewGuid(),
                FeatureId = Guid.NewGuid(),
                TargetType = OverrideType.Group,
                TargetId = "group-1",
                State = false
            }
        );

        await context.SaveChangesAsync();

        // Act
        var result = await repo.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Update_ShouldModifyOverrideState()
    {
        using var context = CreateDbContext();
        var repo = new FeatureOverrideRepository(context);

        var overrideEntity = new FeatureOverride
        {
            Id = Guid.NewGuid(),
            FeatureId = Guid.NewGuid(),
            TargetType = OverrideType.Region,
            TargetId = "IN",
            State = false
        };

        context.FeatureOverrides.Add(overrideEntity);
        await context.SaveChangesAsync();

        // Act
        overrideEntity.State = true;
        repo.Update(overrideEntity);
        await repo.SaveChangesAsync();

        // Assert
        var updated = await context.FeatureOverrides.FindAsync(overrideEntity.Id);
        updated!.State.Should().BeTrue();
    }

    [Fact]
    public async Task Remove_ShouldDeleteOverride()
    {
        using var context = CreateDbContext();
        var repo = new FeatureOverrideRepository(context);

        var overrideEntity = new FeatureOverride
        {
            Id = Guid.NewGuid(),
            FeatureId = Guid.NewGuid(),
            TargetType = OverrideType.User,
            TargetId = "user-delete",
            State = true
        };

        context.FeatureOverrides.Add(overrideEntity);
        await context.SaveChangesAsync();

        // Act
        repo.Remove(overrideEntity);
        await repo.SaveChangesAsync();

        // Assert
        var deleted = await context.FeatureOverrides.FindAsync(overrideEntity.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task GetByFeatureIdAsync_ShouldReturnOnlyMatchingOverrides()
    {
        using var context = CreateDbContext();
        var repo = new FeatureOverrideRepository(context);

        var featureId1 = Guid.NewGuid();
        var featureId2 = Guid.NewGuid();

        context.FeatureOverrides.AddRange(
            new FeatureOverride
            {
                Id = Guid.NewGuid(),
                FeatureId = featureId1,
                TargetType = OverrideType.User,
                TargetId = "user-1",
                State = true
            },
            new FeatureOverride
            {
                Id = Guid.NewGuid(),
                FeatureId = featureId1,
                TargetType = OverrideType.Group,
                TargetId = "group-1",
                State = false
            },
            new FeatureOverride
            {
                Id = Guid.NewGuid(),
                FeatureId = featureId2,
                TargetType = OverrideType.User,
                TargetId = "user-2",
                State = true
            }
        );

        await context.SaveChangesAsync();

        // Act
        var result = await repo.GetByFeatureIdAsync(featureId1);

        // Assert
        result.Should().HaveCount(2);
        result.All(o => o.FeatureId == featureId1).Should().BeTrue();
    }
}

