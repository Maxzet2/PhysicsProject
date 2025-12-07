namespace PhysicsProject.Api.Contracts;

public sealed record StudyPlanResponse(Guid? UserId, IReadOnlyList<StudyPlanChapterDto> Chapters);

public sealed record StudyPlanChapterDto(Guid Id, string Title, string Description, int OrderIndex, IReadOnlyList<StudyPlanSectionDto> Sections);

public sealed record StudyPlanSectionDto(
    Guid Id,
    Guid ChapterId,
    string Title,
    string Description,
    int OrderIndex,
    int DefaultQuestionCount,
    int TestTimeLimitSeconds,
    SectionProgressDto? Progress);

public sealed record SectionProgressDto(
    int AttemptCycle,
    int AttemptsUsedInCycle,
    int AttemptsRemaining,
    int MaxAttemptsPerCycle,
    bool HasAttemptsAvailable,
    Guid? ActiveTestSessionId,
    DateTimeOffset? ActiveTestExpiresAt,
    DateTimeOffset? LastTrainingCompletedAt,
    DateTimeOffset? LastTestPassedAt);
