using Memorio.Flashcards.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Memorio.Flashcards.Infrastructure.Persistence.Configurations;

public sealed class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("Cards");

        builder.HasKey(card => card.Id);

        builder.Property(card => card.DeckId).IsRequired();

        builder.Property(card => card.Front)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(card => card.Back)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(card => card.Tags)
            .HasMaxLength(500);

        builder.Property(card => card.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.HasIndex(card => card.DeckId);

        builder.HasOne<Deck>()
            .WithMany()
            .HasForeignKey(card => card.DeckId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
