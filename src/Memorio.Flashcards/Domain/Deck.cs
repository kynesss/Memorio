using Memorio.Shared.Domain;
using Sieve.Attributes;

namespace Memorio.Flashcards.Domain;

public sealed class Deck : AggregateRoot
{
    private Deck()
    {
    }

    public Guid UserId { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string Name { get; private set; } = default!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string? Description { get; private set; }

    public static Deck Create(Guid userId, string name, string? description) => new()
    {
        UserId = userId,
        Name = name,
        Description = description,
    };

    public void Update(string name, string? description, TimeProvider clock)
    {
        Name = name;
        Description = description;
        MarkUpdated(clock);
    }
}
