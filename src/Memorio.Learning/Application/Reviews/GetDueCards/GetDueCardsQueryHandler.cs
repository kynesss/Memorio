using ErrorOr;
using MediatR;
using Memorio.Learning.Application.Contracts;

namespace Memorio.Learning.Application.Reviews.GetDueCards;

public sealed class GetDueCardsQueryHandler : IRequestHandler<GetDueCardsQuery, ErrorOr<IReadOnlyList<DueCardDto>>>
{
    private readonly DueCardsReader _dueCardsReader;
    private readonly TimeProvider _clock;

    public GetDueCardsQueryHandler(DueCardsReader dueCardsReader, TimeProvider clock)
    {
        _dueCardsReader = dueCardsReader;
        _clock = clock;
    }

    public Task<ErrorOr<IReadOnlyList<DueCardDto>>> Handle(GetDueCardsQuery query, CancellationToken cancellationToken) =>
        _dueCardsReader.GetAsync(query.UserId, query.DeckId, _clock.GetUtcNow().UtcDateTime, cancellationToken);
}
