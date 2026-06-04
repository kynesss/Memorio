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

    public static Task<Card?> FindOwnedCardAsync(
        this FlashcardsDbContext dbContext,
        Guid userId,
        Guid cardId,
        CancellationToken cancellationToken) =>
        dbContext.Cards
            .Include(card => card.MediaItems)
            .FirstOrDefaultAsync(
                card => card.Id == cardId && dbContext.Decks.Any(deck => deck.Id == card.DeckId && deck.UserId == userId),
                cancellationToken);
}
