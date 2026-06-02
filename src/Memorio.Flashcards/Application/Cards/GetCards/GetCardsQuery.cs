using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Shared.Pagination;
using Sieve.Models;

namespace Memorio.Flashcards.Application.Cards.GetCards;

public sealed record GetCardsQuery(Guid UserId, Guid DeckId, SieveModel Sieve) : IRequest<ErrorOr<PagedResult<CardDto>>>;
