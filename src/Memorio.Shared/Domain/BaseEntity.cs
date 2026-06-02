using Sieve.Attributes;

namespace Memorio.Shared.Domain;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime? UpdatedAt { get; protected set; }

    protected void MarkUpdated(TimeProvider clock) => UpdatedAt = clock.GetUtcNow().UtcDateTime;
}
