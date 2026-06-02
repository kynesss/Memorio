namespace Memorio.Learning.Application.Contracts;

public sealed record StudySessionHistoryDto(
    Guid Id,
    Guid DeckId,
    DateTime StartedAt,
    DateTime? CompletedAt,
    int ReviewedCards,
    int TotalReviews,
    int StudyTimeSeconds);
