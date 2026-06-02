using AwesomeAssertions;
using Memorio.Learning.Domain;
using Memorio.Learning.Infrastructure.Scheduling;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace Memorio.Learning.UnitTests.Infrastructure.Scheduling;

public sealed class FsrsReviewSchedulerTests
{
    private static readonly DateTime ReviewedAt = new(2026, 6, 2, 10, 0, 0, DateTimeKind.Utc);

    private readonly FsrsReviewScheduler _scheduler = new();

    [Fact]
    public void Schedule_ReturnsEarlierReviewForAgainThanForEasy()
    {
        var again = _scheduler.Schedule(null, ReviewRating.Again, ReviewedAt, null);
        var easy = _scheduler.Schedule(null, ReviewRating.Easy, ReviewedAt, null);

        again.DueAt.Should().BeBefore(easy.DueAt);
        again.State.Should().Be(LearningState.Learning);
        easy.State.Should().Be(LearningState.Learning);
    }

    [Theory]
    [InlineData(ReviewRating.Again)]
    [InlineData(ReviewRating.Hard)]
    [InlineData(ReviewRating.Good)]
    [InlineData(ReviewRating.Easy)]
    public void Schedule_CalculatesNextReviewForEachRating(ReviewRating rating)
    {
        var schedule = _scheduler.Schedule(null, rating, ReviewedAt, 1200);

        schedule.DueAt.Should().BeAfter(ReviewedAt);
        schedule.LastReviewAt.Should().Be(ReviewedAt);
        schedule.Stability.Should().NotBeNull();
        schedule.Difficulty.Should().NotBeNull();
    }

    [Fact]
    public void Schedule_UsesStoredProgressForSubsequentReview()
    {
        var clock = new FakeTimeProvider(new DateTimeOffset(ReviewedAt));
        var firstSchedule = _scheduler.Schedule(null, ReviewRating.Good, ReviewedAt, null);
        var progress = CardProgress.Create(Guid.NewGuid(), Guid.NewGuid(), firstSchedule);
        var nextReviewAt = firstSchedule.DueAt;

        clock.SetUtcNow(new DateTimeOffset(nextReviewAt));
        var secondSchedule = _scheduler.Schedule(progress, ReviewRating.Good, nextReviewAt, null);
        progress.Apply(secondSchedule, clock);

        progress.ReviewCount.Should().Be(2);
        progress.DueAt.Should().BeAfter(nextReviewAt);
        progress.LastReviewAt.Should().Be(nextReviewAt);
        progress.State.Should().Be(LearningState.Review);
    }
}
