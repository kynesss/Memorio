using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Flashcards.Application.Mapping;
using Memorio.Flashcards.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Flashcards.Application.Decks.GetDeckById;

public sealed class GetDeckByIdQueryHandler : IRequestHandler<GetDeckByIdQuery, ErrorOr<DeckDto>>
{
    private readonly FlashcardsDbContext _dbContext;

    public GetDeckByIdQueryHandler(FlashcardsDbContext dbContext) => _dbContext = dbContext;

    public async Task<ErrorOr<DeckDto>> Handle(GetDeckByIdQuery query, CancellationToken cancellationToken)
    {
        var deck = await _dbContext.Decks
            .AsNoTracking()
            .FirstOrDefaultAsync(deck => deck.Id == query.DeckId && deck.UserId == query.UserId, cancellationToken);

        return deck is null ? FlashcardsErrors.DeckNotFound : deck.ToDto();
    }
}
