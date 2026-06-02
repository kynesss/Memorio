using FluentValidation;
using Memorio.Learning.Application.Abstractions;
using Memorio.Learning.Application.Reviews;
using Memorio.Learning.Infrastructure.Persistence;
using Memorio.Learning.Infrastructure.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Learning;

public static class LearningModule
{
    public static IServiceCollection AddLearningModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LearningDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Database"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", LearningDbContext.Schema)));

        services.AddScoped<IReviewScheduler, FsrsReviewScheduler>();
        services.AddScoped<DueCardsReader>();
        services.AddValidatorsFromAssembly(typeof(LearningModule).Assembly);

        return services;
    }
}
