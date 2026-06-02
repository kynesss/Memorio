using ErrorOr;
using MediatR;
using Memorio.Learning.Application.Contracts;

namespace Memorio.Learning.Application.Reviews.GetDueCards;

public sealed record GetDueCardsQuery(Guid UserId, Guid DeckId) : IRequest<ErrorOr<IReadOnlyList<DueCardDto>>>;
