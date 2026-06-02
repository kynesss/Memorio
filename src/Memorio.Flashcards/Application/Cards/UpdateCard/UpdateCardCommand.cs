using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Flashcards.Domain;

namespace Memorio.Flashcards.Application.Cards.UpdateCard;

public sealed record UpdateCardCommand(
    Guid UserId,
    Guid DeckId,
    Guid CardId,
    string Front,
    string Back,
    string? Tags,
    CardType Type) : IRequest<ErrorOr<CardDto>>;
