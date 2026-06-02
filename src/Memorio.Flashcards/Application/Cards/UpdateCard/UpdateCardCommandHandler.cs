using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Flashcards.Application.Mapping;
using Memorio.Flashcards.Infrastructure.Persistence;

namespace Memorio.Flashcards.Application.Cards.UpdateCard;

public sealed class UpdateCardCommandHandler : IRequestHandler<UpdateCardCommand, ErrorOr<CardDto>>
{
    private readonly FlashcardsDbContext _dbContext;
    private readonly TimeProvider _clock;

    public UpdateCardCommandHandler(FlashcardsDbContext dbContext, TimeProvider clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task<ErrorOr<CardDto>> Handle(UpdateCardCommand command, CancellationToken cancellationToken)
    {
        var deck = await _dbContext.FindDeckAsync(command.UserId, command.DeckId, cancellationToken);
        if (deck is null)
        {
            return FlashcardsErrors.DeckNotFound;
        }

        var card = await _dbContext.FindCardAsync(command.DeckId, command.CardId, cancellationToken);
        if (card is null)
        {
            return FlashcardsErrors.CardNotFound;
        }

        card.Update(command.Front, command.Back, command.Tags, command.Type, _clock);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return card.ToDto();
    }
}
