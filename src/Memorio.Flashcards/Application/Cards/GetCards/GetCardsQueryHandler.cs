using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Flashcards.Application.Mapping;
using Memorio.Flashcards.Infrastructure.Persistence;
using Memorio.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace Memorio.Flashcards.Application.Cards.GetCards;

public sealed class GetCardsQueryHandler : IRequestHandler<GetCardsQuery, ErrorOr<PagedResult<CardDto>>>
{
    private readonly FlashcardsDbContext _dbContext;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly SieveOptions _sieveOptions;

    public GetCardsQueryHandler(
        FlashcardsDbContext dbContext,
        ISieveProcessor sieveProcessor,
        IOptions<SieveOptions> sieveOptions)
    {
        _dbContext = dbContext;
        _sieveProcessor = sieveProcessor;
        _sieveOptions = sieveOptions.Value;
    }

    public async Task<ErrorOr<PagedResult<CardDto>>> Handle(GetCardsQuery query, CancellationToken cancellationToken)
    {
        var deck = await _dbContext.FindDeckAsync(query.UserId, query.DeckId, cancellationToken);
        if (deck is null)
        {
            return FlashcardsErrors.DeckNotFound;
        }

        var cards = _dbContext.Cards
            .AsNoTracking()
            .Where(card => card.DeckId == query.DeckId);

        return await cards.ToPagedResultAsync(query.Sieve, _sieveProcessor, _sieveOptions, card => card.ToDto(), cancellationToken);
    }
}
