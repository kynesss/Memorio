using FsrsSharp;
using Memorio.Learning.Application.Abstractions;
using Memorio.Learning.Domain;
using FsrsCard = FsrsSharp.Card;
using FsrsRating = FsrsSharp.Rating;
using FsrsState = FsrsSharp.State;

namespace Memorio.Learning.Infrastructure.Scheduling;

public sealed class FsrsReviewScheduler : IReviewScheduler
{
    private readonly Scheduler _scheduler = new(enableFuzzing: false);

    public ReviewSchedule Schedule(CardProgress? progress, ReviewRating rating, DateTime reviewedAt, int? reviewDurationMs)
    {
        var reviewTime = new DateTimeOffset(DateTime.SpecifyKind(reviewedAt, DateTimeKind.Utc));
        var result = _scheduler.ReviewCard(ToFsrsCard(progress, reviewTime), ToFsrsRating(rating), reviewTime, reviewDurationMs);

        return new ReviewSchedule(
            result.UpdatedCard.Due.UtcDateTime,
            (LearningState)result.UpdatedCard.State,
            result.UpdatedCard.Step,
            result.UpdatedCard.Stability,
            result.UpdatedCard.Difficulty,
            result.Log.ReviewDateTime.UtcDateTime);
    }

    private static FsrsCard ToFsrsCard(CardProgress? progress, DateTimeOffset reviewTime) =>
        progress is null
            ? new FsrsCard(cardId: 0, due: reviewTime)
            : new FsrsCard(
                cardId: 0,
                state: (FsrsState)progress.State,
                step: progress.Step,
                stability: progress.Stability,
                difficulty: progress.Difficulty,
                due: new DateTimeOffset(DateTime.SpecifyKind(progress.DueAt, DateTimeKind.Utc)),
                lastReview: progress.LastReviewAt is null
                    ? null
                    : new DateTimeOffset(DateTime.SpecifyKind(progress.LastReviewAt.Value, DateTimeKind.Utc)));

    private static FsrsRating ToFsrsRating(ReviewRating rating) => rating switch
    {
        ReviewRating.Again => FsrsRating.Again,
        ReviewRating.Hard => FsrsRating.Hard,
        ReviewRating.Good => FsrsRating.Good,
        ReviewRating.Easy => FsrsRating.Easy,
        _ => throw new ArgumentOutOfRangeException(nameof(rating), rating, null),
    };
}
