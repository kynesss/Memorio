using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Flashcards.Application.Mapping;
using Memorio.Flashcards.Domain;
using Memorio.Flashcards.Infrastructure.Persistence;

namespace Memorio.Flashcards.Application.Decks.CreateDeck;

public sealed class CreateDeckCommandHandler : IRequestHandler<CreateDeckCommand, ErrorOr<DeckDto>>
{
    private readonly FlashcardsDbContext _dbContext;

    public CreateDeckCommandHandler(FlashcardsDbContext dbContext) => _dbContext = dbContext;

    public async Task<ErrorOr<DeckDto>> Handle(CreateDeckCommand command, CancellationToken cancellationToken)
    {
        var deck = Deck.Create(command.UserId, command.Name, command.Description);

        _dbContext.Decks.Add(deck);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return deck.ToDto();
    }
}
