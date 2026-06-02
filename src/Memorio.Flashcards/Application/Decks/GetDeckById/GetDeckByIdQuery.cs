using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Contracts;

namespace Memorio.Flashcards.Application.Decks.GetDeckById;

public sealed record GetDeckByIdQuery(Guid UserId, Guid DeckId) : IRequest<ErrorOr<DeckDto>>;
