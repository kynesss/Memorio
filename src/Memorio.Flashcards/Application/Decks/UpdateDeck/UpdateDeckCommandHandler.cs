using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Flashcards.Application.Mapping;
using Memorio.Flashcards.Infrastructure.Persistence;

namespace Memorio.Flashcards.Application.Decks.UpdateDeck;

public sealed class UpdateDeckCommandHandler : IRequestHandler<UpdateDeckCommand, ErrorOr<DeckDto>>
{
    private readonly FlashcardsDbContext _dbContext;
    private readonly TimeProvider _clock;

    public UpdateDeckCommandHandler(FlashcardsDbContext dbContext, TimeProvider clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task<ErrorOr<DeckDto>> Handle(UpdateDeckCommand command, CancellationToken cancellationToken)
    {
        var deck = await _dbContext.FindDeckAsync(command.UserId, command.DeckId, cancellationToken);
        if (deck is null)
        {
            return FlashcardsErrors.DeckNotFound;
        }

        deck.Update(command.Name, command.Description, _clock);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return deck.ToDto();
    }
}
