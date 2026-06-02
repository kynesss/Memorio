using Memorio.Shared.Domain;

namespace Memorio.Learning.Domain;

public sealed class CardProgress : BaseEntity
{
    private CardProgress()
    {
    }

    public Guid UserId { get; private set; }

    public Guid CardId { get; private set; }

    public DateTime DueAt { get; private set; }

    public LearningState State { get; private set; }

    public int? Step { get; private set; }

    public double? Stability { get; private set; }

    public double? Difficulty { get; private set; }

    public DateTime? LastReviewAt { get; private set; }

    public int ReviewCount { get; private set; }

    public static CardProgress Create(Guid userId, Guid cardId, ReviewSchedule schedule) => new()
    {
        UserId = userId,
        CardId = cardId,
        DueAt = schedule.DueAt,
        State = schedule.State,
        Step = schedule.Step,
        Stability = schedule.Stability,
        Difficulty = schedule.Difficulty,
        LastReviewAt = schedule.LastReviewAt,
        ReviewCount = 1,
    };

    public void Apply(ReviewSchedule schedule, TimeProvider clock)
    {
        DueAt = schedule.DueAt;
        State = schedule.State;
        Step = schedule.Step;
        Stability = schedule.Stability;
        Difficulty = schedule.Difficulty;
        LastReviewAt = schedule.LastReviewAt;
        ReviewCount++;
        MarkUpdated(clock);
    }
}
