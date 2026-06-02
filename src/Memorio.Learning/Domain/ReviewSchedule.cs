namespace Memorio.Learning.Domain;

public sealed record ReviewSchedule(
    DateTime DueAt,
    LearningState State,
    int? Step,
    double? Stability,
    double? Difficulty,
    DateTime LastReviewAt);
