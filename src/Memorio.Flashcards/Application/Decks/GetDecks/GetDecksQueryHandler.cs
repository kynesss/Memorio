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

namespace Memorio.Flashcards.Application.Decks.GetDecks;

public sealed class GetDecksQueryHandler : IRequestHandler<GetDecksQuery, ErrorOr<PagedResult<DeckDto>>>
{
    private readonly FlashcardsDbContext _dbContext;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly SieveOptions _sieveOptions;

    public GetDecksQueryHandler(
        FlashcardsDbContext dbContext,
        ISieveProcessor sieveProcessor,
        IOptions<SieveOptions> sieveOptions)
    {
        _dbContext = dbContext;
        _sieveProcessor = sieveProcessor;
        _sieveOptions = sieveOptions.Value;
    }

    public async Task<ErrorOr<PagedResult<DeckDto>>> Handle(GetDecksQuery query, CancellationToken cancellationToken)
    {
        var decks = _dbContext.Decks
            .AsNoTracking()
            .Where(deck => deck.UserId == query.UserId);

        return await decks.ToPagedResultAsync(query.Sieve, _sieveProcessor, _sieveOptions, deck => deck.ToDto(), cancellationToken);
    }
}
