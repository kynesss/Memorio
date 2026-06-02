using ErrorOr;
using MediatR;
using Memorio.Learning.Application.Contracts;

namespace Memorio.Learning.Application.Sessions.GetHistory;

public sealed record GetStudySessionHistoryQuery(Guid UserId) : IRequest<ErrorOr<IReadOnlyList<StudySessionHistoryDto>>>;
