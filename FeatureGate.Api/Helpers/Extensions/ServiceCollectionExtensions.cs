using FeatureGate.Application.Interfaces.Repositories;
using FeatureGate.Application.Interfaces.Rules;
using FeatureGate.Application.Interfaces.Services.Cache;
using FeatureGate.Application.Interfaces.Services.Features;
using FeatureGate.Application.Mapping;
using FeatureGate.Application.Rules;
using FeatureGate.Application.Rules.Engines;
using FeatureGate.Application.Services.Cache;
using FeatureGate.Application.Services.Features;
using FeatureGate.Application.Validators;
using FeatureGate.Infrastructure.Contexts;
using FeatureGate.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;

namespace FeatureGate.Api.Helpers.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FeatureGateDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("FeatureGate"));
            });

            var redisConnectionString = configuration.GetConnectionString("Redis") ?? throw new ArgumentNullException();

            services.AddSingleton<IConnectionMultiplexer>( sp =>
                ConnectionMultiplexer.Connect(redisConnectionString));

            services.AddScoped<ICacheService, RedisCacheService>();

            services.AddScoped<IFeatureRepository, FeatureRepository>();
            services.AddScoped<IFeatureOverrideRepository, FeatureOverrideRepository>();

            services.AddAutoMapper(a=> a.AddMaps(typeof(FeatureMappingProfile)));

            services.AddValidatorsFromAssemblyContaining<FeatureEvaluationRequestValidator>();


            services.AddScoped<IFeatureRule, UserOverrideRule>();
            services.AddScoped<IFeatureRule, GroupOverrideRule>();
            services.AddScoped<IFeatureRule, RegionOverrideRule>();
            services.AddScoped<IFeatureRule, DefaultRule>();

            services.AddScoped<FeatureRuleEngine>();

            services.AddScoped<IFeatureService, FeatureService>();
            services.AddScoped<IFeatureOverrideService, FeatureOverrideService>();
            services.AddScoped<IFeatureEvaluationService, FeatureEvaluationService>();


            return services;
        }
    }
}
