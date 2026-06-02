using Memorio.Flashcards.Domain;
using Memorio.Learning.Domain;

namespace Memorio.Learning.Application.Contracts;

public sealed record DueCardDto(
    Guid Id,
    Guid DeckId,
    string Front,
    string Back,
    string? Tags,
    CardType Type,
    DateTime DueAt,
    LearningState State,
    double? Stability,
    double? Difficulty);
