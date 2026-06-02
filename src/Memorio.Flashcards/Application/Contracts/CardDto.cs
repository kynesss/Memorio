using Memorio.Flashcards.Domain;

namespace Memorio.Flashcards.Application.Contracts;

public sealed record CardDto(
    Guid Id,
    Guid DeckId,
    string Front,
    string Back,
    string? Tags,
    CardType Type,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
