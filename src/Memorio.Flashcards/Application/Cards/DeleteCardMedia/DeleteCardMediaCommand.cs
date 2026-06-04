using ErrorOr;
using MediatR;

namespace Memorio.Flashcards.Application.Cards.DeleteCardMedia;

public sealed record DeleteCardMediaCommand(
    Guid UserId,
    Guid CardId,
    Guid MediaId) : IRequest<ErrorOr<Deleted>>;
