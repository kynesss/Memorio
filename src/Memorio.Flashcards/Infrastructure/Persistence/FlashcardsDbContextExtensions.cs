using Memorio.Flashcards.Domain;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Flashcards.Infrastructure.Persistence;

internal static class FlashcardsDbContextExtensions
{
    public static Task<Deck?> FindDeckAsync(
        this FlashcardsDbContext dbContext,
        Guid userId,
        Guid deckId,
        CancellationToken cancellationToken) =>
        dbContext.Decks.FirstOrDefaultAsync(deck => deck.Id == deckId && deck.UserId == userId, cancellationToken);

    public static Task<Card?> FindCardAsync(
        this FlashcardsDbContext dbContext,
        Guid deckId,
        Guid cardId,
        CancellationToken cancellationToken) =>
        dbContext.Cards.FirstOrDefaultAsync(card => card.Id == cardId && card.DeckId == deckId, cancellationToken);
}
