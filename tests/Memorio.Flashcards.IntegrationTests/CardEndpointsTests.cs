using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Flashcards.Domain;
using Memorio.Shared.Pagination;
using Xunit;

namespace Memorio.Flashcards.IntegrationTests;

public sealed class CardEndpointsTests : IClassFixture<FlashcardsApiFactory>
{
    private readonly FlashcardsApiFactory _factory;

    public CardEndpointsTests(FlashcardsApiFactory factory) => _factory = factory;

    [Fact]
    public async Task CreateBasicCard_ReturnsSingleCard()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var deck = await CreateDeckAsync(client);

        var response = await client.PostAsJsonAsync($"/api/v1/decks/{deck.Id}/cards", new
        {
            front = "Hello",
            back = "Cześć",
            tags = "english",
            type = CardType.Basic
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var card = await response.Content.ReadFromJsonAsync<CardDto>();
        card!.Front.Should().Be("Hello");
        card.Type.Should().Be(CardType.Basic);

        var cards = await GetCardsAsync(client, deck.Id);
        cards.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task CreateBasicReversedCard_CreatesTwoCards()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var deck = await CreateDeckAsync(client);

        await client.PostAsJsonAsync($"/api/v1/decks/{deck.Id}/cards", new
        {
            front = "Question",
            back = "Answer",
            tags = "english",
            type = CardType.BasicReversed
        });

        var cards = await GetCardsAsync(client, deck.Id);

        cards.TotalCount.Should().Be(2);
        cards.Items.Should().Contain(card => card.Front == "Question" && card.Back == "Answer");
        cards.Items.Should().Contain(card => card.Front == "Answer" && card.Back == "Question");
        cards.Items.Should().OnlyContain(card => card.Type == CardType.Basic);
    }

    [Fact]
    public async Task CreateClozeCard_ReturnsClozeCard()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var deck = await CreateDeckAsync(client);

        var response = await client.PostAsJsonAsync($"/api/v1/decks/{deck.Id}/cards", new
        {
            front = "The capital of France is {{c1::Paris}}",
            back = "Paris",
            tags = "geography",
            type = CardType.Cloze
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var card = await response.Content.ReadFromJsonAsync<CardDto>();
        card!.Type.Should().Be(CardType.Cloze);
    }

    [Fact]
    public async Task CreateCard_ForMissingDeck_ReturnsNotFound()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync($"/api/v1/decks/{Guid.NewGuid()}/cards", new
        {
            front = "x",
            back = "y",
            tags = (string?)null,
            type = CardType.Basic
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCards_WithTagFilterAndSort_ReturnsMatchingCards()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var deck = await CreateDeckAsync(client);
        await CreateCardAsync(client, deck.Id, "Cat", "Kot", "english");
        await CreateCardAsync(client, deck.Id, "Perro", "Pies", "spanish");
        await CreateCardAsync(client, deck.Id, "Dog", "Pies", "english grammar");

        var page = await client.GetFromJsonAsync<PagedResult<CardDto>>(
            $"/api/v1/decks/{deck.Id}/cards?filters=tags@=english&sorts=-createdAt&page=1&pageSize=20");

        page.Should().NotBeNull();
        page!.TotalCount.Should().Be(2);
        page.Page.Should().Be(1);
        page.PageSize.Should().Be(20);
        page.Items.Should().OnlyContain(card => card.Tags!.Contains("english"));
    }

    [Fact]
    public async Task UpdateCard_ChangesFields()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var deck = await CreateDeckAsync(client);
        var card = await CreateCardAsync(client, deck.Id, "Front", "Back", "tag");

        var response = await client.PutAsJsonAsync($"/api/v1/decks/{deck.Id}/cards/{card.Id}", new
        {
            front = "Updated front",
            back = "Updated back",
            tags = "updated",
            type = CardType.Cloze
        });

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<CardDto>();
        updated!.Front.Should().Be("Updated front");
        updated.Type.Should().Be(CardType.Cloze);
        updated.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteCard_RemovesCard()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var deck = await CreateDeckAsync(client);
        var card = await CreateCardAsync(client, deck.Id, "Front", "Back", "tag");

        var deleteResponse = await client.DeleteAsync($"/api/v1/decks/{deck.Id}/cards/{card.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/api/v1/decks/{deck.Id}/cards/{card.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task<DeckDto> CreateDeckAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/v1/decks", new { name = "Deck", description = (string?)null });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<DeckDto>())!;
    }

    private static async Task<CardDto> CreateCardAsync(HttpClient client, Guid deckId, string front, string back, string tags)
    {
        var response = await client.PostAsJsonAsync($"/api/v1/decks/{deckId}/cards", new
        {
            front,
            back,
            tags,
            type = CardType.Basic
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CardDto>())!;
    }

    private static async Task<PagedResult<CardDto>> GetCardsAsync(HttpClient client, Guid deckId)
    {
        var page = await client.GetFromJsonAsync<PagedResult<CardDto>>($"/api/v1/decks/{deckId}/cards");
        return page!;
    }
}
