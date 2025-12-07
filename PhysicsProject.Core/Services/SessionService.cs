using System.Linq;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Services;

public sealed class SessionService : ISessionService
{
    private readonly ISessionRepository _sessions;
    private readonly ITemplateService _templates;
    private readonly IProblemRepository _problems;
    private readonly IEnumerable<IAnswerEvaluator> _evaluators;

    public SessionService(ISessionRepository sessions, ITemplateService templates, IProblemRepository problems, IEnumerable<IAnswerEvaluator> evaluators)
    {
        _sessions = sessions;
        _templates = templates;
        _problems = problems;
        _evaluators = evaluators;
    }

    public async Task<TestSession> CreateSessionAsync(Guid userId, IReadOnlyList<SessionItem> items, string mode, Guid? sectionId, CancellationToken ct)
    {
        var session = new TestSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SectionId = sectionId,
            StartedAt = DateTimeOffset.UtcNow,
            Items = items,
            Mode = mode
        };
        await _sessions.SaveAsync(session, ct);
        return session;
    }

    public async Task<TestSession> CreateSessionFromTemplateAsync(Guid userId, Guid templateId, int numItems, string mode, Guid? sectionId, CancellationToken ct)
    {
        var items = new List<SessionItem>();
        for (int i = 0; i < numItems; i++)
        {
            var instance = await _templates.GenerateInstanceAsync(templateId, null, new GenerationConstraints(), ct);
            items.Add(new SessionItem
            {
                Id = Guid.NewGuid(),
                InstanceId = instance.Id,
                OrderIndex = i + 1,
                MaxScore = 1m
            });
        }
        return await CreateSessionAsync(userId, items, mode, sectionId, ct);
    }

    public async Task<SubmissionResult> SubmitAnswerAsync(Guid sessionId, Guid itemId, string answer, CancellationToken ct)
    {
        var session = await _sessions.GetByIdAsync(sessionId, ct) ?? throw new InvalidOperationException("Session not found");
        if (session.FinishedAt is not null) throw new InvalidOperationException("Session already finished");

        var item = session.Items.FirstOrDefault(i => i.Id == itemId) ?? throw new InvalidOperationException("Session item not found");
        var instance = await _problems.GetInstanceByIdAsync(item.InstanceId, ct) ?? throw new InvalidOperationException("Problem instance not found");
        var evaluator = _evaluators.FirstOrDefault(e => e.CanHandle(instance.TemplateType)) ?? _evaluators.First();

        var evaluation = evaluator.Evaluate(instance, new UserAnswer(answer));
        if (evaluation.ScoreAwarded == 0 && evaluation.IsCorrect)
        {
            evaluation = evaluation with { ScoreAwarded = item.MaxScore };
        }
        else if (evaluation.ScoreAwarded == 0 && evaluation.IsCorrect == false)
        {
            evaluation = evaluation with { ScoreAwarded = 0m };
        }

        var submission = session.RecordSubmission(item.Id, answer, evaluation);
        await _sessions.SaveAsync(session, ct);

        var total = session.Submissions.Count;
        var correct = session.Submissions.Count(s => s.IsCorrect);
        return new SubmissionResult(submission.IsCorrect, submission.ScoreAwarded, submission.Feedback, total, correct);
    }

    public async Task<FinishSessionResult> FinishSessionAsync(Guid sessionId, CancellationToken ct)
    {
        var session = await _sessions.GetByIdAsync(sessionId, ct) ?? throw new InvalidOperationException("Session not found");
        session.Finish();
        await _sessions.SaveAsync(session, ct);
        var correct = session.Submissions.Count(s => s.IsCorrect);
        var total = session.Submissions.Count;
        var totalScore = session.Submissions.Sum(s => s.ScoreAwarded);
        return new FinishSessionResult(totalScore, correct, total);
    }
}


