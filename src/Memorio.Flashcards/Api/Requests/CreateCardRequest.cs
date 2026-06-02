using Memorio.Flashcards.Domain;

namespace Memorio.Flashcards.Api.Requests;

public sealed record CreateCardRequest(string Front, string Back, string? Tags, CardType Type);
