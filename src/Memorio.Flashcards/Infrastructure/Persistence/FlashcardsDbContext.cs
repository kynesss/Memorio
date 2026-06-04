using Memorio.Flashcards.Domain;
using Memorio.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Flashcards.Infrastructure.Persistence;

public sealed class FlashcardsDbContext : DbContext
{
    public const string Schema = "flashcards";

    public FlashcardsDbContext(DbContextOptions<FlashcardsDbContext> options) : base(options)
    {
    }

    public DbSet<Deck> Decks => Set<Deck>();

    public DbSet<Card> Cards => Set<Card>();

    public DbSet<CardMediaItem> CardMediaItems => Set<CardMediaItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(Schema);
        builder.Ignore<DomainEvent>();
        builder.ApplyConfigurationsFromAssembly(typeof(FlashcardsDbContext).Assembly);
    }
}
