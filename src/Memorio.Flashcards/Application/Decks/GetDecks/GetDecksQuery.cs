using ErrorOr;
using MediatR;
using Memorio.Flashcards.Application.Contracts;
using Memorio.Shared.Pagination;
using Sieve.Models;

namespace Memorio.Flashcards.Application.Decks.GetDecks;

public sealed record GetDecksQuery(Guid UserId, SieveModel Sieve) : IRequest<ErrorOr<PagedResult<DeckDto>>>;
