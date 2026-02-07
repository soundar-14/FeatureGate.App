using FeatureGate.Application.Services.Cache;
using FluentAssertions;
using Moq;
using StackExchange.Redis;
using System.Net;
using System.Text.Json;

namespace FeatureGate.Tests.Cache
{
    public class RedisCacheServiceTests
    {
        private readonly Mock<IConnectionMultiplexer> _redisMock;
        private readonly Mock<IDatabase> _dbMock;
        private readonly Mock<IServer> _serverMock;
        private readonly RedisCacheService _cacheService;

        public RedisCacheServiceTests()
        {
            _redisMock = new Mock<IConnectionMultiplexer>();
            _dbMock = new Mock<IDatabase>();
            _serverMock = new Mock<IServer>();

            _redisMock
                .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(_dbMock.Object);

            _redisMock
                .Setup(r => r.GetEndPoints(It.IsAny<bool>()))
                .Returns(new[] { new DnsEndPoint("localhost", 6379) });

            _redisMock
                .Setup(r => r.GetServer(It.IsAny<EndPoint>(), It.IsAny<object>()))
                .Returns(_serverMock.Object);

            _cacheService = new RedisCacheService(_redisMock.Object);
        }

        // ============================
        // GetAsync
        // ============================

        [Fact]
        public async Task GetAsync_WhenKeyExists_ReturnsDeserializedObject()
        {
            // Arrange
            var key = "feature:test";
            var expected = new TestDto { Name = "FeatureX", Enabled = true };
            var json = JsonSerializer.Serialize(expected);

            _dbMock
                .Setup(db => db.StringGetAsync(
                    key,
                    It.IsAny<CommandFlags>()))
                .ReturnsAsync(json);

            // Act
            var result = await _cacheService.GetAsync<TestDto>(key);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("FeatureX");
            result.Enabled.Should().BeTrue();
        }

        [Fact]
        public async Task GetAsync_WhenKeyDoesNotExist_ReturnsNull()
        {
            // Arrange
            var key = "missing:key";

            _dbMock
                .Setup(db => db.StringGetAsync(
                    key,
                    It.IsAny<CommandFlags>()))
                .ReturnsAsync(RedisValue.Null);

            // Act
            var result = await _cacheService.GetAsync<TestDto>(key);

            // Assert
            result.Should().BeNull();
        }

        // ============================
        // SetAsync
        // ============================

        [Fact]
        public async Task SetAsync_ShouldSerializeAndStoreValue()
        {
            // Arrange
            var key = "feature:set";
            var value = new TestDto { Name = "FeatureY", Enabled = false };
            var ttl = TimeSpan.FromMinutes(5);

            _dbMock
                .Setup(db => db.StringSetAsync(
                    key,
                    It.IsAny<RedisValue>(),
                    ttl,
                    It.IsAny<When>(),
                    It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);

            // Act
            await _cacheService.SetAsync(key, value, ttl);

            // Assert
            key.Should().Be("feature:set");
        }

        // ============================
        // RemoveByPrefixAsync
        // ============================

        [Fact]
        public async Task RemoveByPrefixAsync_ShouldDeleteAllMatchingKeys()
        {
            // Arrange
            var prefix = "feature:";
            var keys = new RedisKey[]
            {
                "feature:1",
                "feature:2"
            };

            _serverMock
                .Setup(s => s.Keys(
                    It.IsAny<int>(),
                    $"{prefix}*",
                    It.IsAny<int>(),
                    It.IsAny<long>(),
                    It.IsAny<int>(),
                    It.IsAny<CommandFlags>()))
                .Returns(keys);

            _dbMock
                .Setup(db => db.KeyDeleteAsync(
                    It.IsAny<RedisKey>(),
                    It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);

            // Act
            await _cacheService.RemoveByPrefixAsync(prefix);

            // Assert
            _dbMock.Verify(
                db => db.KeyDeleteAsync("feature:1", It.IsAny<CommandFlags>()),
                Times.Once);

            _dbMock.Verify(
                db => db.KeyDeleteAsync("feature:2", It.IsAny<CommandFlags>()),
                Times.Once);
        }

        // ============================
        // Test DTO
        // ============================

        private class TestDto
        {
            public string Name { get; set; } = string.Empty;
            public bool Enabled { get; set; }
        }
    }
}
