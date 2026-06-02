using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Shared.Pagination;
using Xunit;

namespace Memorio.Flashcards.IntegrationTests;

public sealed class DeckEndpointsTests : IClassFixture<FlashcardsApiFactory>
{
    private readonly FlashcardsApiFactory _factory;

    public DeckEndpointsTests(FlashcardsApiFactory factory) => _factory = factory;

    [Fact]
    public async Task CreateDeck_ReturnsCreatedDeck()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/v1/decks", new { name = "English", description = "Vocabulary" });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var deck = await response.Content.ReadFromJsonAsync<DeckDto>();
        deck.Should().NotBeNull();
        deck!.Name.Should().Be("English");
        deck.Description.Should().Be("Vocabulary");
        deck.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateDeck_WithEmptyName_ReturnsBadRequest()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/v1/decks", new { name = "", description = "x" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetDecks_ReturnsOnlyOwnDecks()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        await CreateDeckAsync(client, "Deck A");
        await CreateDeckAsync(client, "Deck B");

        var page = await client.GetFromJsonAsync<PagedResult<DeckDto>>("/api/v1/decks?page=1&pageSize=20");

        page.Should().NotBeNull();
        page!.TotalCount.Should().Be(2);
        page.Items.Should().OnlyContain(deck => deck.Name == "Deck A" || deck.Name == "Deck B");
    }

    [Fact]
    public async Task GetDeckById_ReturnsDeck()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var created = await CreateDeckAsync(client, "History");

        var deck = await client.GetFromJsonAsync<DeckDto>($"/api/v1/decks/{created.Id}");

        deck.Should().NotBeNull();
        deck!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetDeckById_ReturnsNotFound_WhenMissing()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();

        var response = await client.GetAsync($"/api/v1/decks/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDeck_ChangesFields()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var created = await CreateDeckAsync(client, "Old name");

        var response = await client.PutAsJsonAsync($"/api/v1/decks/{created.Id}", new { name = "New name", description = "Updated" });

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<DeckDto>();
        updated!.Name.Should().Be("New name");
        updated.Description.Should().Be("Updated");
        updated.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteDeck_RemovesDeck()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var created = await CreateDeckAsync(client, "Disposable");

        var deleteResponse = await client.DeleteAsync($"/api/v1/decks/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/api/v1/decks/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDecks_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/decks");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private static async Task<DeckDto> CreateDeckAsync(HttpClient client, string name)
    {
        var response = await client.PostAsJsonAsync("/api/v1/decks", new { name, description = (string?)null });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<DeckDto>())!;
    }
}
