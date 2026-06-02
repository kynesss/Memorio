using MediatR;
using Memorio.Learning.Api.Requests;
using Memorio.Learning.Application.Contracts;
using Memorio.Learning.Application.Reviews.GetDueCards;
using Memorio.Learning.Application.Sessions.Complete;
using Memorio.Learning.Application.Sessions.GetHistory;
using Memorio.Learning.Application.Sessions.Review;
using Memorio.Learning.Application.Sessions.Start;
using Memorio.Learning.Application.Stats.GetUserStats;
using Memorio.Shared.Results;
using Memorio.Shared.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Learning.Api;

public static class LearningEndpoints
{
    public static IEndpointRouteBuilder MapLearningEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var deckGroup = endpoints.MapGroup("/api/v1/decks/{deckId:guid}")
            .WithTags("Learning")
            .RequireAuthorization();

        deckGroup.MapGet("/reviews/due", async (Guid deckId, IUserContext user, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetDueCardsQuery(user.RequireUserId(), deckId), cancellationToken);
            return result.ToResponse();
        })
        .WithName("GetDueCards")
        .WithSummary("Returns cards due for review.")
        .Produces<IReadOnlyList<DueCardDto>>()
        .Produces(StatusCodes.Status404NotFound);

        deckGroup.MapPost("/sessions/start", async (Guid deckId, IUserContext user, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new StartStudySessionCommand(user.RequireUserId(), deckId), cancellationToken);
            return result.ToResponse(session => Results.Created($"/api/v1/sessions/{session.Id}", session));
        })
        .WithName("StartStudySession")
        .WithSummary("Starts a study session and returns due cards.")
        .Produces<StudySessionDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status404NotFound);

        var sessionGroup = endpoints.MapGroup("/api/v1/sessions")
            .WithTags("Learning")
            .RequireAuthorization();

        sessionGroup.MapGet("/", async (IUserContext user, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetStudySessionHistoryQuery(user.RequireUserId()), cancellationToken);
            return result.ToResponse();
        })
        .WithName("GetStudySessionHistory")
        .WithSummary("Returns study session history.")
        .Produces<IReadOnlyList<StudySessionHistoryDto>>();

        sessionGroup.MapPost("/{sessionId:guid}/review", async (
            Guid sessionId,
            ReviewCardRequest request,
            IUserContext user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new ReviewCardCommand(user.RequireUserId(), sessionId, request.CardId, request.Rating, request.ReviewDurationMs);
            var result = await sender.Send(command, cancellationToken);
            return result.ToResponse();
        })
        .WithName("ReviewCard")
        .WithSummary("Records a review and schedules the next repetition.")
        .Produces<CardReviewDto>()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict)
        .ProducesValidationProblem();

        sessionGroup.MapPost("/{sessionId:guid}/complete", async (
            Guid sessionId,
            IUserContext user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new CompleteStudySessionCommand(user.RequireUserId(), sessionId), cancellationToken);
            return result.ToResponse();
        })
        .WithName("CompleteStudySession")
        .WithSummary("Completes a study session and returns its summary.")
        .Produces<StudySessionSummaryDto>()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        endpoints.MapGet("/api/v1/stats", async (IUserContext user, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetUserStatsQuery(user.RequireUserId()), cancellationToken);
            return result.ToResponse();
        })
        .WithTags("Learning")
        .RequireAuthorization()
        .WithName("GetUserStats")
        .WithSummary("Returns learning statistics.")
        .Produces<UserStatsDto>();

        return endpoints;
    }
}
