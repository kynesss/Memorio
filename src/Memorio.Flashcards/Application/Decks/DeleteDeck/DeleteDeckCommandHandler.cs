using ErrorOr;
using MediatR;
using Memorio.Flashcards.Infrastructure.Persistence;

namespace Memorio.Flashcards.Application.Decks.DeleteDeck;

public sealed class DeleteDeckCommandHandler : IRequestHandler<DeleteDeckCommand, ErrorOr<Deleted>>
{
    private readonly FlashcardsDbContext _dbContext;

    public DeleteDeckCommandHandler(FlashcardsDbContext dbContext) => _dbContext = dbContext;

    public async Task<ErrorOr<Deleted>> Handle(DeleteDeckCommand command, CancellationToken cancellationToken)
    {
        var deck = await _dbContext.FindDeckAsync(command.UserId, command.DeckId, cancellationToken);
        if (deck is null)
        {
            return FlashcardsErrors.DeckNotFound;
        }

        _dbContext.Decks.Remove(deck);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
