using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Contracts;

namespace Memorio.Flashcards.Application.Decks.UpdateDeck;

public sealed record UpdateDeckCommand(Guid UserId, Guid DeckId, string Name, string? Description) : IRequest<ErrorOr<DeckDto>>;
