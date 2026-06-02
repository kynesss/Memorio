namespace Memorio.Learning.Application.Contracts;

public sealed record UserStatsDto(
    int CurrentStreakDays,
    double MasteryPercentage,
    int TotalStudyTimeSeconds,
    int CompletedSessions,
    int TotalReviews);
