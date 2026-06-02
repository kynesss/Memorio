using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Contracts;

namespace Memorio.Flashcards.Application.Decks.CreateDeck;

public sealed record CreateDeckCommand(Guid UserId, string Name, string? Description) : IRequest<ErrorOr<DeckDto>>;
