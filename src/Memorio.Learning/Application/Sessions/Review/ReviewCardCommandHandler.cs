using ErrorOr;
using MediatR;
using Memorio.Flashcards.Infrastructure.Persistence;
using Memorio.Learning.Application.Abstractions;
using Memorio.Learning.Application.Contracts;
using Memorio.Learning.Domain;
using Memorio.Learning.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Learning.Application.Sessions.Review;

public sealed class ReviewCardCommandHandler : IRequestHandler<ReviewCardCommand, ErrorOr<CardReviewDto>>
{
    private readonly LearningDbContext _dbContext;
    private readonly FlashcardsDbContext _flashcardsDbContext;
    private readonly IReviewScheduler _scheduler;
    private readonly TimeProvider _clock;

    public ReviewCardCommandHandler(
        LearningDbContext dbContext,
        FlashcardsDbContext flashcardsDbContext,
        IReviewScheduler scheduler,
        TimeProvider clock)
    {
        _dbContext = dbContext;
        _flashcardsDbContext = flashcardsDbContext;
        _scheduler = scheduler;
        _clock = clock;
    }

    public async Task<ErrorOr<CardReviewDto>> Handle(ReviewCardCommand command, CancellationToken cancellationToken)
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

        var cardExists = await _flashcardsDbContext.Cards
            .AnyAsync(card => card.Id == command.CardId && card.DeckId == session.DeckId, cancellationToken);

        if (!cardExists)
        {
            return LearningErrors.CardNotFound;
        }

        var progress = await _dbContext.CardProgresses
            .FirstOrDefaultAsync(
                progress => progress.UserId == command.UserId && progress.CardId == command.CardId,
                cancellationToken);

        var reviewedAt = _clock.GetUtcNow().UtcDateTime;
        var schedule = _scheduler.Schedule(progress, command.Rating, reviewedAt, command.ReviewDurationMs);

        if (progress is null)
        {
            progress = CardProgress.Create(command.UserId, command.CardId, schedule);
            _dbContext.CardProgresses.Add(progress);
        }
        else
        {
            progress.Apply(schedule, _clock);
        }

        var review = CardReview.Create(
            session.Id,
            command.CardId,
            command.Rating,
            reviewedAt,
            schedule.DueAt,
            command.ReviewDurationMs);

        _dbContext.CardReviews.Add(review);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CardReviewDto(
            command.CardId,
            command.Rating,
            reviewedAt,
            schedule.DueAt,
            schedule.State,
            schedule.Stability,
            schedule.Difficulty);
    }
}
