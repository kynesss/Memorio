using ErrorOr;

namespace Memorio.Learning.Application;

internal static class LearningErrors
{
    public static Error DeckNotFound => Error.NotFound("Deck.NotFound", "Deck was not found.");

    public static Error CardNotFound => Error.NotFound("Card.NotFound", "Card was not found.");

    public static Error SessionNotFound => Error.NotFound("StudySession.NotFound", "Study session was not found.");

    public static Error SessionCompleted => Error.Conflict("StudySession.Completed", "Study session is already completed.");
}
