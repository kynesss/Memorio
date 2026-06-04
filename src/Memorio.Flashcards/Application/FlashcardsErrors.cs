using ErrorOr;

namespace Memorio.Flashcards.Application;

internal static class FlashcardsErrors
{
    public static Error DeckNotFound => Error.NotFound("Deck.NotFound", "Deck was not found.");

    public static Error CardNotFound => Error.NotFound("Card.NotFound", "Card was not found.");

    public static Error CardMediaNotFound => Error.NotFound("CardMedia.NotFound", "Card media was not found.");

    public static Error CardMediaFileMissing => Error.Validation("CardMedia.FileMissing", "Image file is required.");

    public static Error CardMediaFileTooLarge => Error.Validation("CardMedia.FileTooLarge", "Image file cannot exceed 10 MB.");

    public static Error CardMediaUnsupportedType => Error.Validation("CardMedia.UnsupportedType", "Image file must be jpg, png, gif or webp.");
}
