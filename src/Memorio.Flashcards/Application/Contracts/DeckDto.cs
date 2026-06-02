namespace Memorio.Flashcards.Application.Contracts;

public sealed record DeckDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
