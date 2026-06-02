using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Flashcards.Application.Mapping;
using Memorio.Flashcards.Domain;
using Memorio.Flashcards.Infrastructure.Persistence;

namespace Memorio.Flashcards.Application.Cards.CreateCard;

public sealed class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, ErrorOr<CardDto>>
{
    private readonly FlashcardsDbContext _dbContext;

    public CreateCardCommandHandler(FlashcardsDbContext dbContext) => _dbContext = dbContext;

    public async Task<ErrorOr<CardDto>> Handle(CreateCardCommand command, CancellationToken cancellationToken)
    {
        var deck = await _dbContext.FindDeckAsync(command.UserId, command.DeckId, cancellationToken);
        if (deck is null)
        {
            return FlashcardsErrors.DeckNotFound;
        }

        var cards = BuildCards(command);

        _dbContext.Cards.AddRange(cards);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return cards[0].ToDto();
    }

    private static IReadOnlyList<Card> BuildCards(CreateCardCommand command) =>
        command.Type == CardType.BasicReversed
            ?
            [
                Card.Create(command.DeckId, command.Front, command.Back, command.Tags, CardType.Basic),
                Card.Create(command.DeckId, command.Back, command.Front, command.Tags, CardType.Basic)
            ]
            : [Card.Create(command.DeckId, command.Front, command.Back, command.Tags, command.Type)];
}
