using Memorio.Learning.Domain;

namespace Memorio.Learning.Application.Abstractions;

public interface IReviewScheduler
{
    ReviewSchedule Schedule(CardProgress? progress, ReviewRating rating, DateTime reviewedAt, int? reviewDurationMs);
}
