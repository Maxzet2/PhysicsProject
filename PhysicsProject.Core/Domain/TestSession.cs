using System.Collections.Generic;
using PhysicsProject.Core.Abstractions;

namespace PhysicsProject.Core.Domain;

public sealed class TestSession
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid? SectionId { get; init; }
    public string Mode { get; init; } = "Normal";
    public DateTimeOffset StartedAt { get; init; }
    public DateTimeOffset? FinishedAt { get; private set; }
    public IReadOnlyList<SessionItem> Items { get; init; } = Array.Empty<SessionItem>();
    public List<Submission> Submissions { get; } = new();

    public void Finish() => FinishedAt = DateTimeOffset.UtcNow;

    public void RestoreFinishedAt(DateTimeOffset? finishedAt) => FinishedAt = finishedAt;

    public void ReplaceSubmissions(IEnumerable<Submission> submissions)
    {
        Submissions.Clear();
        Submissions.AddRange(submissions);
    }

    public Submission RecordSubmission(Guid sessionItemId, string rawAnswer, EvaluationResult evaluation)
    {
        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            SessionItemId = sessionItemId,
            SubmittedAt = DateTimeOffset.UtcNow,
            RawAnswer = rawAnswer,
            IsCorrect = evaluation.IsCorrect,
            ScoreAwarded = evaluation.ScoreAwarded,
            Feedback = evaluation.Feedback
        };
        Submissions.Add(submission);
        return submission;
    }
}

public sealed class SessionItem
{
    public Guid Id { get; init; }
    public Guid InstanceId { get; init; }
    public int OrderIndex { get; init; }
    public decimal MaxScore { get; init; }
}

public sealed class Submission
{
    public Guid Id { get; init; }
    public Guid SessionItemId { get; init; }
    public DateTimeOffset SubmittedAt { get; init; }
    public string RawAnswer { get; init; } = string.Empty;
    public bool IsCorrect { get; init; }
    public decimal ScoreAwarded { get; init; }
    public string Feedback { get; init; } = string.Empty;
}


