using FeatureGate.Api.Helpers.Extensions;
using FeatureGate.Api.Helpers.Middleware;
using System.Diagnostics.CodeAnalysis;

namespace FeatureGate.Api;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddApiServices(builder.Configuration);

        var app = builder.Build();

        await app.MigrateDatabaseAsync();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseAuthorization();

        app.MapControllers();

        app.MapGet("/", context =>
        {
            context.Response.Redirect("/swagger");
            return Task.CompletedTask;
        });

        await app.RunAsync();

    }
}
