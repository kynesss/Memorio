using ErrorOr;
using MediatR;
using Memorio.Flashcards.Infrastructure.Persistence;

namespace Memorio.Flashcards.Application.Cards.DeleteCard;

public sealed class DeleteCardCommandHandler : IRequestHandler<DeleteCardCommand, ErrorOr<Deleted>>
{
    private readonly FlashcardsDbContext _dbContext;

    public DeleteCardCommandHandler(FlashcardsDbContext dbContext) => _dbContext = dbContext;

    public async Task<ErrorOr<Deleted>> Handle(DeleteCardCommand command, CancellationToken cancellationToken)
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

        _dbContext.Cards.Remove(card);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
