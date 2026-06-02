using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Contracts;

namespace Memorio.Flashcards.Application.Cards.GetCardById;

public sealed record GetCardByIdQuery(Guid UserId, Guid DeckId, Guid CardId) : IRequest<ErrorOr<CardDto>>;
