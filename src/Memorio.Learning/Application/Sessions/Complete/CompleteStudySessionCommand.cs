using ErrorOr;
using MediatR;
using Memorio.Learning.Application.Contracts;

namespace Memorio.Learning.Application.Sessions.Complete;

public sealed record CompleteStudySessionCommand(Guid UserId, Guid SessionId) : IRequest<ErrorOr<StudySessionSummaryDto>>;
