using Microsoft.AspNetCore.Mvc;
using PhysicsProject.Api.Contracts;
using PhysicsProject.Core.Abstractions;

namespace PhysicsProject.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly ISessionRepository _sessionRepository;
    private readonly IProblemRepository _problemRepository;
    private readonly ISectionFlowService _sectionFlowService;

    public SessionsController(ISessionService sessionService, ISessionRepository sessionRepository, IProblemRepository problemRepository, ISectionFlowService sectionFlowService)
    {
        _sessionService = sessionService;
        _sessionRepository = sessionRepository;
        _problemRepository = problemRepository;
        _sectionFlowService = sectionFlowService;
    }

    [HttpPost]
    public async Task<ActionResult<CreateSessionResponse>> CreateSession(CreateSessionRequest request, CancellationToken ct)
    {
        var session = await _sessionService.CreateSessionFromTemplateAsync(
            request.UserId,
            request.TemplateId,
            request.NumItems,
            request.Mode,
            request.SectionId,
            ct
        );
        return Ok(new CreateSessionResponse(session.Id, session.StartedAt, session.Items.Count, session.SectionId));
    }

    [HttpGet("{sessionId:guid}")]
    public async Task<ActionResult<SessionDto>> GetSession(Guid sessionId, CancellationToken ct)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, ct);
        if (session is null) return NotFound();

        var items = new List<SessionItemDto>();
        foreach (var item in session.Items)
        {
            var instance = await _problemRepository.GetInstanceByIdAsync(item.InstanceId, ct);
            if (instance is null) continue;

            var submitted = session.Submissions.Any(s => s.SessionItemId == item.Id);

            items.Add(new SessionItemDto(
                item.Id,
                item.InstanceId,
                item.OrderIndex,
                item.MaxScore,
                instance.Statement,
                submitted
            ));
        }

        return Ok(new SessionDto(
            session.Id,
            session.UserId,
            session.SectionId,
            session.Mode,
            session.StartedAt,
            session.FinishedAt,
            items
        ));
    }

    [HttpPost("{sessionId:guid}/items/{itemId:guid}/submit")]
    public async Task<ActionResult<SubmitAnswerResponse>> SubmitAnswer(Guid sessionId, Guid itemId, SubmitAnswerRequest request, CancellationToken ct)
    {
        var result = await _sessionService.SubmitAnswerAsync(sessionId, itemId, request.Answer, ct);
        return Ok(new SubmitAnswerResponse(result.IsCorrect, result.ScoreAwarded, result.Feedback, result.TotalSubmissions, result.CorrectSubmissions));
    }

    [HttpPost("{sessionId:guid}/finish")]
    public async Task<ActionResult<FinishSessionResponse>> FinishSession(Guid sessionId, CancellationToken ct)
    {
        var result = await _sessionService.FinishSessionAsync(sessionId, ct);
        var session = await _sessionRepository.GetByIdAsync(sessionId, ct);
        var passed = false;

        if (session?.SectionId is Guid sectionId)
        {
            if (string.Equals(session.Mode, "training", StringComparison.OrdinalIgnoreCase))
            {
                await _sectionFlowService.RecordTrainingCompletionAsync(session.UserId, sectionId, ct);
            }
            else if (string.Equals(session.Mode, "test", StringComparison.OrdinalIgnoreCase))
            {
                passed = session.Items.Count > 0 && result.CorrectAnswers == session.Items.Count;
                await _sectionFlowService.RecordTestCompletionAsync(session.Id, passed, ct);
            }
        }

        return Ok(new FinishSessionResponse(result.TotalScore, result.CorrectAnswers, result.TotalAnswers, passed));
    }
}
