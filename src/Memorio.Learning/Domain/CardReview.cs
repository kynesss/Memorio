using Memorio.Shared.Domain;

namespace Memorio.Learning.Domain;

public sealed class CardReview : BaseEntity
{
    private CardReview()
    {
    }

    public Guid SessionId { get; private set; }

    public Guid CardId { get; private set; }

    public ReviewRating Rating { get; private set; }

    public DateTime ReviewedAt { get; private set; }

    public DateTime NextReviewAt { get; private set; }

    public int? ReviewDurationMs { get; private set; }

    public static CardReview Create(
        Guid sessionId,
        Guid cardId,
        ReviewRating rating,
        DateTime reviewedAt,
        DateTime nextReviewAt,
        int? reviewDurationMs) => new()
        {
            SessionId = sessionId,
            CardId = cardId,
            Rating = rating,
            ReviewedAt = reviewedAt,
            NextReviewAt = nextReviewAt,
            ReviewDurationMs = reviewDurationMs,
        };
}
