using ErrorOr;
using MediatR;
using Memorio.Learning.Application.Contracts;

namespace Memorio.Learning.Application.Stats.GetUserStats;

public sealed record GetUserStatsQuery(Guid UserId) : IRequest<ErrorOr<UserStatsDto>>;
