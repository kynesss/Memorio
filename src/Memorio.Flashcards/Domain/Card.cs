using Memorio.Shared.Domain;
using Sieve.Attributes;

namespace Memorio.Flashcards.Domain;

public sealed class Card : BaseEntity
{
    private readonly List<CardMediaItem> _mediaItems = [];

    private Card()
    {
    }

    public Guid DeckId { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string Front { get; private set; } = default!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string Back { get; private set; } = default!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string? Tags { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public CardType Type { get; private set; }

    public IReadOnlyCollection<CardMediaItem> MediaItems => _mediaItems;

    public static Card Create(Guid deckId, string front, string back, string? tags, CardType type) => new()
    {
        DeckId = deckId,
        Front = front,
        Back = back,
        Tags = tags,
        Type = type,
    };

    public void Update(string front, string back, string? tags, CardType type, TimeProvider clock)
    {
        Front = front;
        Back = back;
        Tags = tags;
        Type = type;
        MarkUpdated(clock);
    }
}
