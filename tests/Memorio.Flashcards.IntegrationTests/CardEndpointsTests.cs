using System.Net;
using System.Net.Http.Headers;
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

    [Fact]
    public async Task UploadCardMedia_AddsMediaToCard()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var deck = await CreateDeckAsync(client);
        var card = await CreateCardAsync(client, deck.Id, "Front", "Back", "tag");

        var response = await UploadPngAsync(client, card.Id);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var media = await response.Content.ReadFromJsonAsync<CardMediaDto>();
        media.Should().NotBeNull();
        media!.Url.Should().Contain($"/users/");
        media.FileName.Should().EndWith(".png");
        media.FileSize.Should().Be(PngImage.Length);

        var cards = await GetCardsAsync(client, deck.Id);
        cards.Items.Single().MediaItems.Should().ContainSingle(item => item.Id == media.Id);
    }

    [Fact]
    public async Task UploadCardMedia_WithUnsupportedFile_ReturnsBadRequest()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var deck = await CreateDeckAsync(client);
        var card = await CreateCardAsync(client, deck.Id, "Front", "Back", "tag");

        using var content = new MultipartFormDataContent();
        var file = new ByteArrayContent("not an image"u8.ToArray());
        file.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(file, "file", "image.png");

        var response = await client.PostAsync($"/api/v1/cards/{card.Id}/media", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadCardMedia_WithTooLargeFile_ReturnsBadRequest()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var deck = await CreateDeckAsync(client);
        var card = await CreateCardAsync(client, deck.Id, "Front", "Back", "tag");
        var bytes = new byte[(10 * 1024 * 1024) + 1];
        bytes[0] = 0xff;
        bytes[1] = 0xd8;
        bytes[2] = 0xff;

        using var content = new MultipartFormDataContent();
        var file = new ByteArrayContent(bytes);
        file.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        content.Add(file, "file", "image.jpg");

        var response = await client.PostAsync($"/api/v1/cards/{card.Id}/media", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCardMedia_RemovesMediaFromCard()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var deck = await CreateDeckAsync(client);
        var card = await CreateCardAsync(client, deck.Id, "Front", "Back", "tag");
        var uploadResponse = await UploadPngAsync(client, card.Id);
        var media = await uploadResponse.Content.ReadFromJsonAsync<CardMediaDto>();

        var deleteResponse = await client.DeleteAsync($"/api/v1/cards/{card.Id}/media/{media!.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var cards = await GetCardsAsync(client, deck.Id);
        cards.Items.Single().MediaItems.Should().BeEmpty();
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

    private static Task<HttpResponseMessage> UploadPngAsync(HttpClient client, Guid cardId)
    {
        var content = new MultipartFormDataContent();
        var file = new ByteArrayContent(PngImage);
        file.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(file, "file", "image.png");
        return client.PostAsync($"/api/v1/cards/{cardId}/media", content);
    }

    private static readonly byte[] PngImage =
    [
        0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a,
        0x00, 0x00, 0x00, 0x0d
    ];
}
