using Memorio.Learning.Domain;

namespace Memorio.Learning.Api.Requests;

public sealed record ReviewCardRequest(Guid CardId, ReviewRating Rating, int? ReviewDurationMs);
