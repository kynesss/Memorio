using ErrorOr;
using MediatR;
using Memorio.Learning.Application.Contracts;
using Memorio.Learning.Domain;

namespace Memorio.Learning.Application.Sessions.Review;

public sealed record ReviewCardCommand(
    Guid UserId,
    Guid SessionId,
    Guid CardId,
    ReviewRating Rating,
    int? ReviewDurationMs) : IRequest<ErrorOr<CardReviewDto>>;
