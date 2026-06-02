using MediatR;
using Memorio.Flashcards.Api.Requests;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Flashcards.Application.Decks.CreateDeck;
using Memorio.Flashcards.Application.Decks.DeleteDeck;
using Memorio.Flashcards.Application.Decks.GetDeckById;
using Memorio.Flashcards.Application.Decks.GetDecks;
using Memorio.Flashcards.Application.Decks.UpdateDeck;
using Memorio.Shared.Pagination;
using Memorio.Shared.Results;
using Memorio.Shared.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sieve.Models;

namespace Memorio.Flashcards.Api;

internal static class DeckEndpoints
{
    public static IEndpointRouteBuilder MapDeckEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/decks")
            .WithTags("Decks")
            .RequireAuthorization();

        group.MapGet("/", async ([AsParameters] SieveModel sieve, IUserContext user, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetDecksQuery(user.RequireUserId(), sieve), cancellationToken);
            return result.ToResponse();
        })
        .WithName("GetDecks")
        .WithSummary("Zwraca stronicowaną listę decków zalogowanego użytkownika.")
        .Produces<PagedResult<DeckDto>>();

        group.MapGet("/{deckId:guid}", async (Guid deckId, IUserContext user, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetDeckByIdQuery(user.RequireUserId(), deckId), cancellationToken);
            return result.ToResponse();
        })
        .WithName("GetDeckById")
        .WithSummary("Zwraca pojedynczy deck.")
        .Produces<DeckDto>()
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateDeckRequest request, IUserContext user, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new CreateDeckCommand(user.RequireUserId(), request.Name, request.Description), cancellationToken);
            return result.ToResponse(deck => Results.Created($"/api/v1/decks/{deck.Id}", deck));
        })
        .WithName("CreateDeck")
        .WithSummary("Tworzy nowy deck.")
        .Produces<DeckDto>(StatusCodes.Status201Created)
        .ProducesValidationProblem();

        group.MapPut("/{deckId:guid}", async (Guid deckId, UpdateDeckRequest request, IUserContext user, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new UpdateDeckCommand(user.RequireUserId(), deckId, request.Name, request.Description), cancellationToken);
            return result.ToResponse();
        })
        .WithName("UpdateDeck")
        .WithSummary("Aktualizuje deck.")
        .Produces<DeckDto>()
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();

        group.MapDelete("/{deckId:guid}", async (Guid deckId, IUserContext user, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new DeleteDeckCommand(user.RequireUserId(), deckId), cancellationToken);
            return result.ToResponse(_ => Results.NoContent());
        })
        .WithName("DeleteDeck")
        .WithSummary("Usuwa deck wraz z jego kartami.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }
}
