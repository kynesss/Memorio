using Memorio.Learning.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Memorio.Learning.Infrastructure.Persistence.Configurations;

public sealed class CardProgressConfiguration : IEntityTypeConfiguration<CardProgress>
{
    public void Configure(EntityTypeBuilder<CardProgress> builder)
    {
        builder.ToTable("CardProgresses");

        builder.HasKey(progress => progress.Id);

        builder.Property(progress => progress.UserId).IsRequired();
        builder.Property(progress => progress.CardId).IsRequired();
        builder.Property(progress => progress.DueAt).IsRequired();
        builder.Property(progress => progress.State).IsRequired().HasConversion<string>().HasMaxLength(16);
        builder.Property(progress => progress.ReviewCount).IsRequired();

        builder.HasIndex(progress => progress.UserId);
        builder.HasIndex(progress => progress.CardId);
        builder.HasIndex(progress => new { progress.UserId, progress.CardId }).IsUnique();
        builder.HasIndex(progress => new { progress.UserId, progress.DueAt });
    }
}
