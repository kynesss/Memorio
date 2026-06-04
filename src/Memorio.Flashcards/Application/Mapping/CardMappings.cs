using Memorio.Flashcards.Application.Contracts;
using Memorio.Flashcards.Domain;

namespace Memorio.Flashcards.Application.Mapping;

internal static class CardMappings
{
    public static CardDto ToDto(this Card card) =>
        new(
            card.Id,
            card.DeckId,
            card.Front,
            card.Back,
            card.Tags,
            card.Type,
            card.CreatedAt,
            card.UpdatedAt,
            card.MediaItems
                .OrderBy(media => media.CreatedAt)
                .Select(media => media.ToDto())
                .ToList());

    public static CardMediaDto ToDto(this CardMediaItem media) =>
        new(media.Id, media.Url, media.FileName, media.FileSize, media.CreatedAt);
}
