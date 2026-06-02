using ErrorOr;
using Memorio.Flashcards.Infrastructure.Persistence;
using Memorio.Learning.Application.Contracts;
using Memorio.Learning.Domain;
using Memorio.Learning.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Learning.Application.Reviews;

public sealed class DueCardsReader
{
    private readonly FlashcardsDbContext _flashcardsDbContext;
    private readonly LearningDbContext _learningDbContext;

    public DueCardsReader(FlashcardsDbContext flashcardsDbContext, LearningDbContext learningDbContext)
    {
        _flashcardsDbContext = flashcardsDbContext;
        _learningDbContext = learningDbContext;
    }

    public async Task<ErrorOr<IReadOnlyList<DueCardDto>>> GetAsync(
        Guid userId,
        Guid deckId,
        DateTime currentTime,
        CancellationToken cancellationToken)
    {
        var deckExists = await _flashcardsDbContext.Decks
            .AnyAsync(deck => deck.Id == deckId && deck.UserId == userId, cancellationToken);

        if (!deckExists)
        {
            return LearningErrors.DeckNotFound;
        }

        var cards = await _flashcardsDbContext.Cards
            .AsNoTracking()
            .Where(card => card.DeckId == deckId)
            .ToListAsync(cancellationToken);

        var cardIds = cards.Select(card => card.Id).ToList();
        var progresses = await _learningDbContext.CardProgresses
            .AsNoTracking()
            .Where(progress => progress.UserId == userId && cardIds.Contains(progress.CardId))
            .ToDictionaryAsync(progress => progress.CardId, cancellationToken);

        var tomorrow = currentTime.Date.AddDays(1);

        return cards
            .Where(card => !progresses.TryGetValue(card.Id, out var progress) || progress.DueAt < tomorrow)
            .Select(card =>
            {
                progresses.TryGetValue(card.Id, out var progress);

                return new DueCardDto(
                    card.Id,
                    card.DeckId,
                    card.Front,
                    card.Back,
                    card.Tags,
                    card.Type,
                    progress?.DueAt ?? currentTime,
                    progress?.State ?? LearningState.New,
                    progress?.Stability,
                    progress?.Difficulty);
            })
            .OrderBy(card => card.DueAt)
            .ToList();
    }
}
