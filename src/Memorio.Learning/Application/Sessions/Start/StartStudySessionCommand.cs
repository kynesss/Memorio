using ErrorOr;
using MediatR;
using Memorio.Learning.Application.Contracts;

namespace Memorio.Learning.Application.Sessions.Start;

public sealed record StartStudySessionCommand(Guid UserId, Guid DeckId) : IRequest<ErrorOr<StudySessionDto>>;
