namespace PhysicsProject.Api.Contracts;

public sealed record StartSectionSessionRequest(Guid UserId);

public sealed record SectionSessionResponse(
    Guid SessionId,
    Guid SectionId,
    string Mode,
    DateTimeOffset StartedAt,
    DateTimeOffset? ExpiresAt,
    int QuestionCount,
    SectionProgressDto? Progress);

public sealed record ResetSectionAttemptsRequest(Guid UserId);

public sealed record ResetSectionAttemptsResponse(
    Guid SectionId,
    int AttemptCycle,
    int AttemptsRemaining,
    int MaxAttemptsPerCycle,
    bool HasAttemptsAvailable);

public sealed record SectionProgressResponse(Guid SectionId, SectionProgressDto? Progress);
