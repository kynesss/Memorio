using Memorio.Flashcards.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Memorio.Flashcards.Infrastructure.Persistence.Configurations;

public sealed class DeckConfiguration : IEntityTypeConfiguration<Deck>
{
    public void Configure(EntityTypeBuilder<Deck> builder)
    {
        builder.ToTable("Decks");

        builder.HasKey(deck => deck.Id);

        builder.Property(deck => deck.UserId).IsRequired();

        builder.Property(deck => deck.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(deck => deck.Description)
            .HasMaxLength(2000);

        builder.HasIndex(deck => deck.UserId);
    }
}
