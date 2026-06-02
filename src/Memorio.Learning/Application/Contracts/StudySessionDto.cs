namespace Memorio.Learning.Application.Contracts;

public sealed record StudySessionDto(
    Guid Id,
    Guid DeckId,
    DateTime StartedAt,
    IReadOnlyList<DueCardDto> DueCards);
