using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Abstractions;
using Memorio.Flashcards.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Flashcards.Application.Cards.DeleteCardMedia;

public sealed class DeleteCardMediaCommandHandler : IRequestHandler<DeleteCardMediaCommand, ErrorOr<Deleted>>
{
    private readonly FlashcardsDbContext _dbContext;
    private readonly ICardMediaStorage _storage;

    public DeleteCardMediaCommandHandler(FlashcardsDbContext dbContext, ICardMediaStorage storage)
    {
        _dbContext = dbContext;
        _storage = storage;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteCardMediaCommand command, CancellationToken cancellationToken)
    {
        var card = await _dbContext.FindOwnedCardAsync(command.UserId, command.CardId, cancellationToken);
        if (card is null)
        {
            return FlashcardsErrors.CardNotFound;
        }

        var media = await _dbContext.CardMediaItems
            .FirstOrDefaultAsync(
                media => media.Id == command.MediaId && media.CardId == command.CardId,
                cancellationToken);

        if (media is null)
        {
            return FlashcardsErrors.CardMediaNotFound;
        }

        await _storage.DeleteAsync(media.ObjectKey, cancellationToken);

        _dbContext.CardMediaItems.Remove(media);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
