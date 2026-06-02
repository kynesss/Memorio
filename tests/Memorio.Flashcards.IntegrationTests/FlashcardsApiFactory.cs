using System.Net.Http.Headers;
using System.Net.Http.Json;
using Memorio.Users.Application.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Xunit;

namespace Memorio.Flashcards.IntegrationTests;

public sealed class FlashcardsApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder("postgres:16-alpine")
        .Build();

    public Task InitializeAsync() => _database.StartAsync();

    public new async Task DisposeAsync()
    {
        await _database.DisposeAsync();
        await base.DisposeAsync();
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            email = $"user-{Guid.NewGuid():N}@memorio.test",
            password = "Sup3rSecret!"
        });
        response.EnsureSuccessStatusCode();

        var tokens = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);

        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("ConnectionStrings:Database", _database.GetConnectionString());
        builder.UseSetting("Jwt:Secret", "integration-tests-signing-secret-key-0123456789");
        builder.UseSetting("Jwt:Issuer", "memorio-tests");
        builder.UseSetting("Jwt:Audience", "memorio-tests");
    }
}
