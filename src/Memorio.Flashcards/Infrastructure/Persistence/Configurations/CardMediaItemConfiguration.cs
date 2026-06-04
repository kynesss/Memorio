using Memorio.Flashcards.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Memorio.Flashcards.Infrastructure.Persistence.Configurations;

public sealed class CardMediaItemConfiguration : IEntityTypeConfiguration<CardMediaItem>
{
    public void Configure(EntityTypeBuilder<CardMediaItem> builder)
    {
        builder.ToTable("CardMediaItems");

        builder.HasKey(media => media.Id);

        builder.Property(media => media.CardId)
            .IsRequired();

        builder.Property(media => media.Url)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(media => media.ObjectKey)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(media => media.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(media => media.FileSize)
            .IsRequired();

        builder.HasIndex(media => media.CardId);
        builder.HasIndex(media => media.ObjectKey).IsUnique();
    }
}
