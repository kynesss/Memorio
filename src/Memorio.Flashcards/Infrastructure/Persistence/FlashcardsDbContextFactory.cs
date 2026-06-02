using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Memorio.Flashcards.Infrastructure.Persistence;

public sealed class FlashcardsDbContextFactory : IDesignTimeDbContextFactory<FlashcardsDbContext>
{
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=memorio_db;Username=memorio;Password=change_me_in_production";

    public FlashcardsDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Database")
            ?? DefaultConnectionString;

        var options = new DbContextOptionsBuilder<FlashcardsDbContext>()
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, FlashcardsDbContext.Schema))
            .Options;

        return new FlashcardsDbContext(options);
    }
}
