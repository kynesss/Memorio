using Memorio.Shared.Domain;

namespace Memorio.Flashcards.Domain;

public sealed class CardMediaItem : BaseEntity
{
    private CardMediaItem()
    {
    }

    public Guid CardId { get; private set; }

    public string Url { get; private set; } = default!;

    public string ObjectKey { get; private set; } = default!;

    public string FileName { get; private set; } = default!;

    public long FileSize { get; private set; }

    public static CardMediaItem Create(
        Guid cardId,
        string url,
        string objectKey,
        string fileName,
        long fileSize,
        TimeProvider clock) => new()
        {
            CardId = cardId,
            Url = url,
            ObjectKey = objectKey,
            FileName = fileName,
            FileSize = fileSize,
            CreatedAt = clock.GetUtcNow().UtcDateTime
        };
}
