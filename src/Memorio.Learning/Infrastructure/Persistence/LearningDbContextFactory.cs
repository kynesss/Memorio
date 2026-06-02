using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Memorio.Learning.Infrastructure.Persistence;

public sealed class LearningDbContextFactory : IDesignTimeDbContextFactory<LearningDbContext>
{
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=memorio_db;Username=memorio;Password=change_me_in_production";

    public LearningDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Database")
            ?? DefaultConnectionString;

        var options = new DbContextOptionsBuilder<LearningDbContext>()
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, LearningDbContext.Schema))
            .Options;

        return new LearningDbContext(options);
    }
}
