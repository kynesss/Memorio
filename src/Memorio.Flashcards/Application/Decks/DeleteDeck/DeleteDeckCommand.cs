using ErrorOr;
using MediatR;

namespace Memorio.Flashcards.Application.Decks.DeleteDeck;

public sealed record DeleteDeckCommand(Guid UserId, Guid DeckId) : IRequest<ErrorOr<Deleted>>;
