using Memorio.Learning.Domain;
using Memorio.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Learning.Infrastructure.Persistence;

public sealed class LearningDbContext : DbContext
{
    public const string Schema = "learning";

    public LearningDbContext(DbContextOptions<LearningDbContext> options) : base(options)
    {
    }

    public DbSet<StudySession> StudySessions => Set<StudySession>();

    public DbSet<CardReview> CardReviews => Set<CardReview>();

    public DbSet<CardProgress> CardProgresses => Set<CardProgress>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(Schema);
        builder.Ignore<DomainEvent>();
        builder.ApplyConfigurationsFromAssembly(typeof(LearningDbContext).Assembly);
    }
}
