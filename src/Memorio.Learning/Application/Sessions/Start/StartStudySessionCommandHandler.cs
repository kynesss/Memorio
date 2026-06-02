using ErrorOr;
using MediatR;
using Memorio.Learning.Application.Contracts;
using Memorio.Learning.Application.Reviews;
using Memorio.Learning.Domain;
using Memorio.Learning.Infrastructure.Persistence;

namespace Memorio.Learning.Application.Sessions.Start;

public sealed class StartStudySessionCommandHandler : IRequestHandler<StartStudySessionCommand, ErrorOr<StudySessionDto>>
{
    private readonly LearningDbContext _dbContext;
    private readonly DueCardsReader _dueCardsReader;
    private readonly TimeProvider _clock;

    public StartStudySessionCommandHandler(LearningDbContext dbContext, DueCardsReader dueCardsReader, TimeProvider clock)
    {
        _dbContext = dbContext;
        _dueCardsReader = dueCardsReader;
        _clock = clock;
    }

    public async Task<ErrorOr<StudySessionDto>> Handle(StartStudySessionCommand command, CancellationToken cancellationToken)
    {
        var dueCards = await _dueCardsReader.GetAsync(
            command.UserId,
            command.DeckId,
            _clock.GetUtcNow().UtcDateTime,
            cancellationToken);

        if (dueCards.IsError)
        {
            return dueCards.Errors;
        }

        var session = StudySession.Start(command.UserId, command.DeckId, _clock);
        _dbContext.StudySessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new StudySessionDto(session.Id, session.DeckId, session.StartedAt, dueCards.Value);
    }
}
