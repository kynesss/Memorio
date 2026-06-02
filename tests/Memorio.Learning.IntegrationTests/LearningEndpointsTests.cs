using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Learning.Application.Contracts;
using Memorio.Learning.Domain;
using Xunit;

namespace Memorio.Learning.IntegrationTests;

public sealed class LearningEndpointsTests : IClassFixture<LearningApiFactory>
{
    private readonly LearningApiFactory _factory;

    public LearningEndpointsTests(LearningApiFactory factory) => _factory = factory;

    [Fact]
    public async Task StudySession_CompletesFullLearningFlow()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var deck = await CreateDeckAsync(client);
        var card = await CreateCardAsync(client, deck.Id);

        var startResponse = await client.PostAsync($"/api/v1/decks/{deck.Id}/sessions/start", null);
        startResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var session = await startResponse.Content.ReadFromJsonAsync<StudySessionDto>();
        session.Should().NotBeNull();
        session!.DueCards.Should().ContainSingle(dueCard => dueCard.Id == card.Id);

        var reviewResponse = await client.PostAsJsonAsync($"/api/v1/sessions/{session.Id}/review", new
        {
            cardId = card.Id,
            rating = ReviewRating.Good,
            reviewDurationMs = 1500,
        });
        reviewResponse.EnsureSuccessStatusCode();
        var review = await reviewResponse.Content.ReadFromJsonAsync<CardReviewDto>();
        review.Should().NotBeNull();
        review!.NextReviewAt.Should().BeAfter(review.ReviewedAt);

        var dueCards = await client.GetFromJsonAsync<IReadOnlyList<DueCardDto>>($"/api/v1/decks/{deck.Id}/reviews/due");
        dueCards.Should().ContainSingle(dueCard => dueCard.Id == card.Id);

        var completeResponse = await client.PostAsync($"/api/v1/sessions/{session.Id}/complete", null);
        completeResponse.EnsureSuccessStatusCode();
        var summary = await completeResponse.Content.ReadFromJsonAsync<StudySessionSummaryDto>();
        summary.Should().NotBeNull();
        summary!.ReviewedCards.Should().Be(1);
        summary.TotalReviews.Should().Be(1);

        var stats = await client.GetFromJsonAsync<UserStatsDto>("/api/v1/stats");
        stats.Should().NotBeNull();
        stats!.CurrentStreakDays.Should().Be(1);
        stats.CompletedSessions.Should().Be(1);
        stats.TotalReviews.Should().Be(1);

        var history = await client.GetFromJsonAsync<IReadOnlyList<StudySessionHistoryDto>>("/api/v1/sessions");
        history.Should().ContainSingle(item => item.Id == session.Id && item.TotalReviews == 1);
    }

    private static async Task<DeckDto> CreateDeckAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/v1/decks", new { name = "English", description = "Vocabulary" });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<DeckDto>())!;
    }

    private static async Task<CardDto> CreateCardAsync(HttpClient client, Guid deckId)
    {
        var response = await client.PostAsJsonAsync($"/api/v1/decks/{deckId}/cards", new
        {
            front = "hello",
            back = "czesc",
            tags = "basics",
            type = 0,
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CardDto>())!;
    }
}
