using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AwesomeAssertions;
using Memorio.Users.Application.Contracts;
using Microsoft.AspNetCore.Mvc.Testing;
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
        var tokens = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();

        tokens.Should().NotBeNull();
        tokens!.AccessToken.Should().NotBeNullOrWhiteSpace();
        tokens.TokenType.Should().Be("Bearer");
        tokens.ExpiresInSeconds.Should().Be(900);
        (await response.Content.ReadAsStringAsync()).Should().NotContain("refresh_token");
        response.Headers.GetValues("Set-Cookie").Should().Contain(cookie =>
            cookie.Contains("refresh_token=") && cookie.Contains("httponly", StringComparison.OrdinalIgnoreCase));
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
        var tokens = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();

        tokens.Should().NotBeNull();
        tokens!.AccessToken.Should().NotBeNullOrWhiteSpace();
        (await response.Content.ReadAsStringAsync()).Should().NotContain("refresh_token");
        response.Headers.GetValues("Set-Cookie").Should().Contain(cookie =>
            cookie.Contains("refresh_token=") && cookie.Contains("httponly", StringComparison.OrdinalIgnoreCase));
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
        var registerResponse = await RegisterAsync(client, UniqueEmail(), "Sup3rSecret!");
        var originalRefreshToken = GetRefreshTokenCookie(registerResponse);

        var response = await client.PostAsync("/api/v1/auth/refresh", null);

        response.EnsureSuccessStatusCode();
        var refreshed = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();

        refreshed.Should().NotBeNull();
        refreshed!.AccessToken.Should().NotBeNullOrWhiteSpace();
        (await response.Content.ReadAsStringAsync()).Should().NotContain("refresh_token");
        GetRefreshTokenCookie(response).Should().NotBe(originalRefreshToken);
    }

    [Fact]
    public async Task Refresh_WithRevokedToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });
        var registerResponse = await RegisterAsync(client, UniqueEmail(), "Sup3rSecret!");
        var refreshToken = GetRefreshTokenCookie(registerResponse);

        var refresh = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/refresh");
        refresh.Headers.Add("Cookie", $"refresh_token={refreshToken}");
        var refreshResponse = await client.SendAsync(refresh);
        refreshResponse.EnsureSuccessStatusCode();

        var reuse = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/refresh");
        reuse.Headers.Add("Cookie", $"refresh_token={refreshToken}");
        var reuseResponse = await client.SendAsync(reuse);

        reuseResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_WithoutCookie_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });

        var response = await client.PostAsync("/api/v1/auth/refresh", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCurrentUser_WithAccessToken_ReturnsUser()
    {
        var client = _factory.CreateClient();
        var email = UniqueEmail();
        var registerResponse = await RegisterAsync(client, email, "Sup3rSecret!");
        var tokens = await registerResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);

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

    private static async Task<HttpResponseMessage> RegisterAsync(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new { email, password });
        response.EnsureSuccessStatusCode();
        return response;
    }

    private static string GetRefreshTokenCookie(HttpResponseMessage response) =>
        response.Headers.GetValues("Set-Cookie")
            .Single(cookie => cookie.StartsWith("refresh_token=", StringComparison.Ordinal))
            .Split(';', 2)[0]
            .Split('=', 2)[1];

    private static string UniqueEmail() => $"user-{Guid.NewGuid():N}@memorio.test";
}
