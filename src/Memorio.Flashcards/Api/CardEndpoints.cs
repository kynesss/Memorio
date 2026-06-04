using MediatR;
using Memorio.Flashcards.Api.Requests;
using Memorio.Flashcards.Application.Cards.CreateCard;
using Memorio.Flashcards.Application.Cards.DeleteCard;
using Memorio.Flashcards.Application.Cards.DeleteCardMedia;
using Memorio.Flashcards.Application.Cards.GetCardById;
using Memorio.Flashcards.Application.Cards.GetCards;
using Memorio.Flashcards.Application.Cards.UploadCardMedia;
using Memorio.Flashcards.Application.Cards.UpdateCard;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Shared.Pagination;
using Memorio.Shared.Results;
using Memorio.Shared.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sieve.Models;

namespace Memorio.Flashcards.Api;

internal static class CardEndpoints
{
    public static IEndpointRouteBuilder MapCardEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/decks/{deckId:guid}/cards")
            .WithTags("Cards")
            .RequireAuthorization();

        endpoints.MapPost("/api/v1/cards/{cardId:guid}/media", async (Guid cardId, IFormFile file, IUserContext user, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new UploadCardMediaCommand(user.RequireUserId(), cardId, file), cancellationToken);
            return result.ToResponse(media => Results.Created($"/api/v1/cards/{cardId}/media/{media.Id}", media));
        })
        .WithName("UploadCardMedia")
        .WithTags("Cards")
        .WithSummary("Dodaje zdjęcie do karty.")
        .RequireAuthorization()
        .DisableAntiforgery()
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<CardMediaDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();

        endpoints.MapDelete("/api/v1/cards/{cardId:guid}/media/{mediaId:guid}", async (Guid cardId, Guid mediaId, IUserContext user, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new DeleteCardMediaCommand(user.RequireUserId(), cardId, mediaId), cancellationToken);
            return result.ToResponse(_ => Results.NoContent());
        })
        .WithName("DeleteCardMedia")
        .WithTags("Cards")
        .WithSummary("Usuwa zdjęcie z karty.")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (Guid deckId, [AsParameters] SieveModel sieve, IUserContext user, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCardsQuery(user.RequireUserId(), deckId, sieve), cancellationToken);
            return result.ToResponse();
        })
        .WithName("GetCards")
        .WithSummary("Zwraca stronicowaną listę kart w decku z filtrowaniem i sortowaniem (Sieve).")
        .Produces<PagedResult<CardDto>>()
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{cardId:guid}", async (Guid deckId, Guid cardId, IUserContext user, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCardByIdQuery(user.RequireUserId(), deckId, cardId), cancellationToken);
            return result.ToResponse();
        })
        .WithName("GetCardById")
        .WithSummary("Zwraca pojedynczą kartę.")
        .Produces<CardDto>()
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (Guid deckId, CreateCardRequest request, IUserContext user, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateCardCommand(user.RequireUserId(), deckId, request.Front, request.Back, request.Tags, request.Type);
            var result = await sender.Send(command, cancellationToken);
            return result.ToResponse(card => Results.Created($"/api/v1/decks/{deckId}/cards/{card.Id}", card));
        })
        .WithName("CreateCard")
        .WithSummary("Tworzy kartę (typ BasicReversed tworzy dodatkowo kartę odwróconą).")
        .Produces<CardDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();

        group.MapPut("/{cardId:guid}", async (Guid deckId, Guid cardId, UpdateCardRequest request, IUserContext user, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new UpdateCardCommand(user.RequireUserId(), deckId, cardId, request.Front, request.Back, request.Tags, request.Type);
            var result = await sender.Send(command, cancellationToken);
            return result.ToResponse();
        })
        .WithName("UpdateCard")
        .WithSummary("Aktualizuje kartę.")
        .Produces<CardDto>()
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();

        group.MapDelete("/{cardId:guid}", async (Guid deckId, Guid cardId, IUserContext user, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new DeleteCardCommand(user.RequireUserId(), deckId, cardId), cancellationToken);
            return result.ToResponse(_ => Results.NoContent());
        })
        .WithName("DeleteCard")
        .WithSummary("Usuwa kartę.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }
}
