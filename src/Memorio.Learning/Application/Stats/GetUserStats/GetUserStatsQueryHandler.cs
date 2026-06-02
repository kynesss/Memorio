using ErrorOr;
using MediatR;
using Memorio.Flashcards.Infrastructure.Persistence;
using Memorio.Learning.Application.Contracts;
using Memorio.Learning.Domain;
using Memorio.Learning.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Learning.Application.Stats.GetUserStats;

public sealed class GetUserStatsQueryHandler : IRequestHandler<GetUserStatsQuery, ErrorOr<UserStatsDto>>
{
    private readonly LearningDbContext _dbContext;
    private readonly FlashcardsDbContext _flashcardsDbContext;
    private readonly TimeProvider _clock;

    public GetUserStatsQueryHandler(
        LearningDbContext dbContext,
        FlashcardsDbContext flashcardsDbContext,
        TimeProvider clock)
    {
        _dbContext = dbContext;
        _flashcardsDbContext = flashcardsDbContext;
        _clock = clock;
    }

    public async Task<ErrorOr<UserStatsDto>> Handle(GetUserStatsQuery query, CancellationToken cancellationToken)
    {
        var cardIds = await (
            from card in _flashcardsDbContext.Cards
            join deck in _flashcardsDbContext.Decks on card.DeckId equals deck.Id
            where deck.UserId == query.UserId
            select card.Id)
            .ToListAsync(cancellationToken);

        var masteredCards = await _dbContext.CardProgresses
            .CountAsync(
                progress => progress.UserId == query.UserId
                    && cardIds.Contains(progress.CardId)
                    && progress.State == LearningState.Review,
                cancellationToken);

        var completedSessions = await _dbContext.StudySessions
            .AsNoTracking()
            .Where(session => session.UserId == query.UserId && session.CompletedAt != null)
            .ToListAsync(cancellationToken);

        var reviewDates = await (
            from review in _dbContext.CardReviews
            join session in _dbContext.StudySessions on review.SessionId equals session.Id
            where session.UserId == query.UserId
            select review.ReviewedAt.Date)
            .Distinct()
            .ToListAsync(cancellationToken);

        var totalReviews = await (
            from review in _dbContext.CardReviews
            join session in _dbContext.StudySessions on review.SessionId equals session.Id
            where session.UserId == query.UserId
            select review.Id)
            .CountAsync(cancellationToken);

        return new UserStatsDto(
            CalculateStreak(reviewDates, _clock.GetUtcNow().UtcDateTime.Date),
            cardIds.Count == 0 ? 0 : Math.Round(masteredCards * 100d / cardIds.Count, 2),
            completedSessions.Sum(session => (int)(session.CompletedAt!.Value - session.StartedAt).TotalSeconds),
            completedSessions.Count,
            totalReviews);
    }

    private static int CalculateStreak(IReadOnlyCollection<DateTime> reviewDates, DateTime today)
    {
        var dates = reviewDates.ToHashSet();
        var date = dates.Contains(today) ? today : today.AddDays(-1);
        var streak = 0;

        while (dates.Contains(date))
        {
            streak++;
            date = date.AddDays(-1);
        }

        return streak;
    }
}
