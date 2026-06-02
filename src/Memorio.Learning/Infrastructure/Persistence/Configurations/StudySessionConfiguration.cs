using Memorio.Learning.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Memorio.Learning.Infrastructure.Persistence.Configurations;

public sealed class StudySessionConfiguration : IEntityTypeConfiguration<StudySession>
{
    public void Configure(EntityTypeBuilder<StudySession> builder)
    {
        builder.ToTable("StudySessions");

        builder.HasKey(session => session.Id);

        builder.Property(session => session.UserId).IsRequired();
        builder.Property(session => session.DeckId).IsRequired();
        builder.Property(session => session.StartedAt).IsRequired();

        builder.Ignore(session => session.IsCompleted);

        builder.HasIndex(session => session.UserId);
        builder.HasIndex(session => session.DeckId);
    }
}
