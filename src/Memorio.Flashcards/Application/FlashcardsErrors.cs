using ErrorOr;

namespace Memorio.Flashcards.Application;

internal static class FlashcardsErrors
{
    public static Error DeckNotFound => Error.NotFound("Deck.NotFound", "Deck was not found.");

    public static Error CardNotFound => Error.NotFound("Card.NotFound", "Card was not found.");
}
