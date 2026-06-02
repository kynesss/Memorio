using Memorio.Learning.Domain;

namespace Memorio.Learning.Application.Contracts;

public sealed record CardReviewDto(
    Guid CardId,
    ReviewRating Rating,
    DateTime ReviewedAt,
    DateTime NextReviewAt,
    LearningState State,
    double? Stability,
    double? Difficulty);
