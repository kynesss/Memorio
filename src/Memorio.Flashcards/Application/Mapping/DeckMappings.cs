using Memorio.Flashcards.Application.Contracts;
using Memorio.Flashcards.Domain;

namespace Memorio.Flashcards.Application.Mapping;

internal static class DeckMappings
{
    public static DeckDto ToDto(this Deck deck) =>
        new(deck.Id, deck.Name, deck.Description, deck.CreatedAt, deck.UpdatedAt);
}
