
using AutoMapper;
using FeatureGate.Application.DTOs.FeatureOverrides;
using FeatureGate.Application.Helpers.Constants;
using FeatureGate.Application.Interfaces.Repositories;
using FeatureGate.Application.Interfaces.Services.Cache;
using FeatureGate.Application.Services.Features;
using FeatureGate.Domain.Entities;
using FeatureGate.Domain.Enums;
using FeatureGate.Infrastructure.Contexts;
using FeatureGate.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

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
    public async Task CreateAsync_Throws_WhenUserOverrideAlreadyExists()
    {
        var repo = new Mock<IFeatureOverrideRepository>();
        var cache = new Mock<ICacheService>();
        var mapper = new Mock<IMapper>();

        repo.Setup(r => r.UserExistsAsync(It.IsAny<Guid>(), "user1"))
            .ReturnsAsync(true);

        var service = new FeatureOverrideService(
            repo.Object, cache.Object, mapper.Object);

        var dto = new FeatureOverrideDto
        {
            FeatureId = Guid.NewGuid(),
            TargetType = OverrideType.User,
            TargetId = "user1"
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenGroupOverrideAlreadyExists()
    {
        var repo = new Mock<IFeatureOverrideRepository>();
        var cache = new Mock<ICacheService>();
        var mapper = new Mock<IMapper>();

        repo.Setup(r => r.GroupExistsAsync(It.IsAny<Guid>(), "group1"))
            .ReturnsAsync(true);

        var service = new FeatureOverrideService(
            repo.Object, cache.Object, mapper.Object);

        var dto = new FeatureOverrideDto
        {
            FeatureId = Guid.NewGuid(),
            TargetType = OverrideType.Group,
            TargetId = "group1"
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenRegionOverrideAlreadyExists()
    {
        var repo = new Mock<IFeatureOverrideRepository>();
        var cache = new Mock<ICacheService>();
        var mapper = new Mock<IMapper>();

        repo.Setup(r => r.RegionExistsAsync(It.IsAny<Guid>(), "IN"))
            .ReturnsAsync(true);

        var service = new FeatureOverrideService(
            repo.Object, cache.Object, mapper.Object);

        var dto = new FeatureOverrideDto
        {
            FeatureId = Guid.NewGuid(),
            TargetType = OverrideType.Region,
            TargetId = "IN"
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_DoesNotThrow_ForUnknownTargetType()
    {
        var repo = new Mock<IFeatureOverrideRepository>();
        var cache = new Mock<ICacheService>();
        var mapper = new Mock<IMapper>();

        mapper.Setup(m => m.Map<FeatureOverride>(It.IsAny<FeatureOverrideDto>()))
              .Returns(new FeatureOverride());

        var service = new FeatureOverrideService(
            repo.Object, cache.Object, mapper.Object);

        var dto = new FeatureOverrideDto
        {
            FeatureId = Guid.NewGuid(),
            TargetType = (OverrideType)999, // hits default
            TargetId = "x"
        };

        await service.CreateAsync(dto);

        cache.Verify(c => c.RemoveByPrefixAsync(CacheKeys.FeatureAllPrefix),
            Times.Once);
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

    [Fact]
    public async Task UserExistsAsync_ReturnsTrue_WhenUserOverrideExists()
    {
        var db = CreateDbContext();
        var repo = new FeatureOverrideRepository(db);

        var featureId = Guid.NewGuid();

        db.FeatureOverrides.Add(new FeatureOverride
        {
            FeatureId = featureId,
            TargetType = OverrideType.User,
            TargetId = "user-123"
        });
        await db.SaveChangesAsync();

        var result = await repo.UserExistsAsync(featureId, "user-123");

        Assert.True(result);
    }

    [Fact]
    public async Task UserExistsAsync_ReturnsFalse_WhenUserOverrideDoesNotExist()
    {
        var db = CreateDbContext();
        var repo = new FeatureOverrideRepository(db);

        var result = await repo.UserExistsAsync(Guid.NewGuid(), "user-123");

        Assert.False(result);
    }

    [Fact]
    public async Task GroupExistsAsync_ReturnsTrue_WhenGroupOverrideExists()
    {
        var db = CreateDbContext();
        var repo = new FeatureOverrideRepository(db);

        var featureId = Guid.NewGuid();

        db.FeatureOverrides.Add(new FeatureOverride
        {
            FeatureId = featureId,
            TargetType = OverrideType.Group,
            TargetId = "group-1"
        });
        await db.SaveChangesAsync();

        var result = await repo.GroupExistsAsync(featureId, "group-1");

        Assert.True(result);
    }

    [Fact]
    public async Task RegionExistsAsync_ReturnsTrue_WhenRegionOverrideExists()
    {
        var db = CreateDbContext();
        var repo = new FeatureOverrideRepository(db);

        var featureId = Guid.NewGuid();

        db.FeatureOverrides.Add(new FeatureOverride
        {
            FeatureId = featureId,
            TargetType = OverrideType.Region,
            TargetId = "IN"
        });
        await db.SaveChangesAsync();

        var result = await repo.RegionExistsAsync(featureId, "IN");

        Assert.True(result);
    }
}

