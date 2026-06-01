using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AwesomeAssertions;
using Memorio.Users.Application.Contracts;
using Xunit;

namespace Memorio.Users.IntegrationTests;

public sealed class AuthEndpointsTests : IClassFixture<AuthApiFactory>
{
    private readonly AuthApiFactory _factory;

    public AuthEndpointsTests(AuthApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Register_WithValidPayload_ReturnsTokenPair()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            email = UniqueEmail(),
            password = "Sup3rSecret!"
        });

        response.EnsureSuccessStatusCode();
        var tokens = await response.Content.ReadFromJsonAsync<AuthResponse>();

        tokens.Should().NotBeNull();
        tokens!.AccessToken.Should().NotBeNullOrWhiteSpace();
        tokens.RefreshToken.Should().NotBeNullOrWhiteSpace();
        tokens.TokenType.Should().Be("Bearer");
        tokens.ExpiresInSeconds.Should().Be(900);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            email = "not-an-email",
            password = "Sup3rSecret!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokenPair()
    {
        var client = _factory.CreateClient();
        var email = UniqueEmail();
        const string password = "Sup3rSecret!";
        await RegisterAsync(client, email, password);

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password });

        response.EnsureSuccessStatusCode();
        var tokens = await response.Content.ReadFromJsonAsync<AuthResponse>();

        tokens.Should().NotBeNull();
        tokens!.AccessToken.Should().NotBeNullOrWhiteSpace();
        tokens.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var email = UniqueEmail();
        await RegisterAsync(client, email, "Sup3rSecret!");

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email,
            password = "WrongPassword!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_WithValidToken_ReturnsNewTokenPair()
    {
        var client = _factory.CreateClient();
        var registered = await RegisterAsync(client, UniqueEmail(), "Sup3rSecret!");

        var response = await client.PostAsJsonAsync("/api/v1/auth/refresh", new
        {
            refresh_token = registered.RefreshToken
        });

        response.EnsureSuccessStatusCode();
        var refreshed = await response.Content.ReadFromJsonAsync<AuthResponse>();

        refreshed.Should().NotBeNull();
        refreshed!.RefreshToken.Should().NotBe(registered.RefreshToken);
    }

    [Fact]
    public async Task Refresh_WithRevokedToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var registered = await RegisterAsync(client, UniqueEmail(), "Sup3rSecret!");

        await client.PostAsJsonAsync("/api/v1/auth/refresh", new { refresh_token = registered.RefreshToken });
        var reuse = await client.PostAsJsonAsync("/api/v1/auth/refresh", new { refresh_token = registered.RefreshToken });

        reuse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCurrentUser_WithAccessToken_ReturnsUser()
    {
        var client = _factory.CreateClient();
        var email = UniqueEmail();
        var registered = await RegisterAsync(client, email, "Sup3rSecret!");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", registered.AccessToken);

        var response = await client.GetAsync("/api/v1/auth/me");

        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();

        user.Should().NotBeNull();
        user!.Email.Should().Be(email);
        user.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetCurrentUser_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private static async Task<AuthResponse> RegisterAsync(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new { email, password });
        response.EnsureSuccessStatusCode();

        var tokens = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return tokens!;
    }

    private static string UniqueEmail() => $"user-{Guid.NewGuid():N}@memorio.test";
}
