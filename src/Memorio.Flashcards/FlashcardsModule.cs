using FluentValidation;
using Memorio.Flashcards.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sieve.Models;
using Sieve.Services;

namespace Memorio.Flashcards;

public static class FlashcardsModule
{
    public static IServiceCollection AddFlashcardsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FlashcardsDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Database"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", FlashcardsDbContext.Schema)));

        services.Configure<SieveOptions>(options =>
        {
            options.CaseSensitive = false;
            options.DefaultPageSize = 20;
            options.MaxPageSize = 100;
        });
        services.AddScoped<ISieveProcessor, SieveProcessor>();

        services.AddValidatorsFromAssembly(typeof(FlashcardsModule).Assembly);

        return services;
    }
}
