using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Flashcards.Domain;

namespace Memorio.Flashcards.Application.Cards.CreateCard;

public sealed record CreateCardCommand(
    Guid UserId,
    Guid DeckId,
    string Front,
    string Back,
    string? Tags,
    CardType Type) : IRequest<ErrorOr<CardDto>>;
