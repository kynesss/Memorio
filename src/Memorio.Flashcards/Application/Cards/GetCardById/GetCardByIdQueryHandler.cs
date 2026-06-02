using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Flashcards.Application.Mapping;
using Memorio.Flashcards.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Flashcards.Application.Cards.GetCardById;

public sealed class GetCardByIdQueryHandler : IRequestHandler<GetCardByIdQuery, ErrorOr<CardDto>>
{
    private readonly FlashcardsDbContext _dbContext;

    public GetCardByIdQueryHandler(FlashcardsDbContext dbContext) => _dbContext = dbContext;

    public async Task<ErrorOr<CardDto>> Handle(GetCardByIdQuery query, CancellationToken cancellationToken)
    {
        var deck = await _dbContext.FindDeckAsync(query.UserId, query.DeckId, cancellationToken);
        if (deck is null)
        {
            return FlashcardsErrors.DeckNotFound;
        }

        var card = await _dbContext.Cards
            .AsNoTracking()
            .FirstOrDefaultAsync(card => card.Id == query.CardId && card.DeckId == query.DeckId, cancellationToken);

        return card is null ? FlashcardsErrors.CardNotFound : card.ToDto();
    }
}
