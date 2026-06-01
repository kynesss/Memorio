using AwesomeAssertions;
using Memorio.Users.Domain;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace Memorio.Users.UnitTests.Domain;

public sealed class RefreshTokenTests
{
    private static readonly DateTimeOffset Now = new(2026, 6, 1, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Issue_SetsExpiryRelativeToCurrentTime()
    {
        var clock = new FakeTimeProvider(Now);

        var token = RefreshToken.Issue(Guid.NewGuid(), TimeSpan.FromDays(7), clock);

        token.ExpiresAt.Should().Be(Now.UtcDateTime.AddDays(7));
        token.Token.Should().NotBeNullOrWhiteSpace();
        token.RevokedAt.Should().BeNull();
    }

    [Fact]
    public void IsActive_IsTrue_WhenNotExpiredAndNotRevoked()
    {
        var clock = new FakeTimeProvider(Now);
        var token = RefreshToken.Issue(Guid.NewGuid(), TimeSpan.FromDays(7), clock);

        token.IsActive(clock).Should().BeTrue();
    }

    [Fact]
    public void IsActive_IsFalse_AfterExpiry()
    {
        var clock = new FakeTimeProvider(Now);
        var token = RefreshToken.Issue(Guid.NewGuid(), TimeSpan.FromDays(7), clock);

        clock.Advance(TimeSpan.FromDays(7) + TimeSpan.FromSeconds(1));

        token.IsActive(clock).Should().BeFalse();
    }

    [Fact]
    public void IsActive_IsFalse_AfterRevoke()
    {
        var clock = new FakeTimeProvider(Now);
        var token = RefreshToken.Issue(Guid.NewGuid(), TimeSpan.FromDays(7), clock);

        token.Revoke(clock);

        token.RevokedAt.Should().Be(Now.UtcDateTime);
        token.IsActive(clock).Should().BeFalse();
    }
}
