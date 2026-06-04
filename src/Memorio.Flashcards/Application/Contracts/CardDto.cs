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
    DateTime? UpdatedAt,
    IReadOnlyList<CardMediaDto> MediaItems);

public sealed record CardMediaDto(
    Guid Id,
    string Url,
    string FileName,
    long FileSize,
    DateTime CreatedAt);
