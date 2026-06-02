using ErrorOr;
using MediatR;

namespace Memorio.Flashcards.Application.Cards.DeleteCard;

public sealed record DeleteCardCommand(Guid UserId, Guid DeckId, Guid CardId) : IRequest<ErrorOr<Deleted>>;
