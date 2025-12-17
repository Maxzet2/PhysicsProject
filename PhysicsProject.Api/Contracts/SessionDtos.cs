namespace PhysicsProject.Api.Contracts;

public record CreateSessionRequest(Guid UserId, Guid TemplateId, int NumItems = 10, string Mode = "Normal", Guid? SectionId = null);
public record CreateSessionResponse(Guid SessionId, DateTimeOffset StartedAt, int Items, Guid? SectionId);

public record SubmitAnswerRequest(string Answer);
public record SubmitAnswerResponse(bool IsCorrect, decimal ScoreAwarded, string Feedback, int TotalSubmissions, int CorrectSubmissions);

public record FinishSessionResponse(decimal TotalScore, int CorrectAnswers, int TotalAnswers, bool Passed);

public record SessionItemDto(
    Guid ItemId,
    Guid InstanceId,
    int Order,
    decimal MaxScore,
    string Statement,
    bool Submitted
);

public record SessionDto(
    Guid Id,
    Guid UserId,
    Guid? SectionId,
    string Mode,
    DateTimeOffset StartedAt,
    DateTimeOffset? FinishedAt,
    IReadOnlyList<SessionItemDto> Items
);
