using Memorio.Shared.Domain;

namespace Memorio.Learning.Domain;

public sealed class StudySession : AggregateRoot
{
    private StudySession()
    {
    }

    public Guid UserId { get; private set; }

    public Guid DeckId { get; private set; }

    public DateTime StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public bool IsCompleted => CompletedAt is not null;

    public static StudySession Start(Guid userId, Guid deckId, TimeProvider clock) => new()
    {
        UserId = userId,
        DeckId = deckId,
        StartedAt = clock.GetUtcNow().UtcDateTime,
    };

    public void Complete(TimeProvider clock) => CompletedAt = clock.GetUtcNow().UtcDateTime;
}
