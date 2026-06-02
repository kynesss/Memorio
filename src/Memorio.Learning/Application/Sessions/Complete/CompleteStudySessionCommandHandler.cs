using ErrorOr;
using MediatR;
using Memorio.Learning.Application.Contracts;
using Memorio.Learning.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Learning.Application.Sessions.Complete;

public sealed class CompleteStudySessionCommandHandler : IRequestHandler<CompleteStudySessionCommand, ErrorOr<StudySessionSummaryDto>>
{
    private readonly LearningDbContext _dbContext;
    private readonly TimeProvider _clock;

    public CompleteStudySessionCommandHandler(LearningDbContext dbContext, TimeProvider clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task<ErrorOr<StudySessionSummaryDto>> Handle(
        CompleteStudySessionCommand command,
        CancellationToken cancellationToken)
    {
        var session = await _dbContext.StudySessions
            .FirstOrDefaultAsync(session => session.Id == command.SessionId && session.UserId == command.UserId, cancellationToken);

        if (session is null)
        {
            return LearningErrors.SessionNotFound;
        }

        if (session.IsCompleted)
        {
            return LearningErrors.SessionCompleted;
        }

        session.Complete(_clock);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var reviewStats = await _dbContext.CardReviews
            .Where(review => review.SessionId == session.Id)
            .GroupBy(_ => 1)
            .Select(reviews => new
            {
                ReviewedCards = reviews.Select(review => review.CardId).Distinct().Count(),
                TotalReviews = reviews.Count(),
            })
            .FirstOrDefaultAsync(cancellationToken);

        return new StudySessionSummaryDto(
            session.Id,
            session.DeckId,
            session.StartedAt,
            session.CompletedAt!.Value,
            reviewStats?.ReviewedCards ?? 0,
            reviewStats?.TotalReviews ?? 0,
            (int)(session.CompletedAt.Value - session.StartedAt).TotalSeconds);
    }
}
