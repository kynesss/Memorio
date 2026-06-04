using Amazon.S3;
using FluentValidation;
using Memorio.Flashcards.Application.Abstractions;
using Memorio.Flashcards.Infrastructure.Persistence;
using Memorio.Flashcards.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

        services.Configure<CardMediaStorageOptions>(configuration.GetSection(CardMediaStorageOptions.SectionName));
        services.AddSingleton<IAmazonS3>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<CardMediaStorageOptions>>().Value;
            return new AmazonS3Client(options.AccessKey, options.SecretKey, new AmazonS3Config
            {
                ServiceURL = options.Endpoint,
                AuthenticationRegion = options.Region,
                ForcePathStyle = options.ForcePathStyle
            });
        });
        services.AddScoped<ICardMediaStorage, S3CardMediaStorage>();

        services.AddValidatorsFromAssembly(typeof(FlashcardsModule).Assembly);

        return services;
    }
}
