using ErrorOr;
using MediatR;
using Memorio.Learning.Application.Contracts;
using Memorio.Learning.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Learning.Application.Sessions.GetHistory;

public sealed class GetStudySessionHistoryQueryHandler
    : IRequestHandler<GetStudySessionHistoryQuery, ErrorOr<IReadOnlyList<StudySessionHistoryDto>>>
{
    private readonly LearningDbContext _dbContext;

    public GetStudySessionHistoryQueryHandler(LearningDbContext dbContext) => _dbContext = dbContext;

    public async Task<ErrorOr<IReadOnlyList<StudySessionHistoryDto>>> Handle(
        GetStudySessionHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var sessions = await _dbContext.StudySessions
            .AsNoTracking()
            .Where(session => session.UserId == query.UserId)
            .OrderByDescending(session => session.StartedAt)
            .ToListAsync(cancellationToken);

        var sessionIds = sessions.Select(session => session.Id).ToList();
        var reviewStats = await _dbContext.CardReviews
            .AsNoTracking()
            .Where(review => sessionIds.Contains(review.SessionId))
            .GroupBy(review => review.SessionId)
            .Select(reviews => new
            {
                SessionId = reviews.Key,
                ReviewedCards = reviews.Select(review => review.CardId).Distinct().Count(),
                TotalReviews = reviews.Count(),
            })
            .ToDictionaryAsync(stats => stats.SessionId, cancellationToken);

        return sessions
            .Select(session =>
            {
                reviewStats.TryGetValue(session.Id, out var stats);

                return new StudySessionHistoryDto(
                    session.Id,
                    session.DeckId,
                    session.StartedAt,
                    session.CompletedAt,
                    stats?.ReviewedCards ?? 0,
                    stats?.TotalReviews ?? 0,
                    session.CompletedAt is null ? 0 : (int)(session.CompletedAt.Value - session.StartedAt).TotalSeconds);
            })
            .ToList();
    }
}
