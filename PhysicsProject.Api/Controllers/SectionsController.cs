using Microsoft.AspNetCore.Mvc;
using PhysicsProject.Api.Contracts;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;

namespace PhysicsProject.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SectionsController : ControllerBase
{
    private readonly ISectionFlowService _sectionFlowService;
    private readonly ISectionProgressRepository _progressRepository;

    public SectionsController(
        ISectionFlowService sectionFlowService,
        ISectionProgressRepository progressRepository)
    {
        _sectionFlowService = sectionFlowService;
        _progressRepository = progressRepository;
    }

    [HttpPost("{sectionId:guid}/training/start")]
    public async Task<ActionResult<SectionSessionResponse>> StartTraining(Guid sectionId, StartSectionSessionRequest request, CancellationToken ct)
    {
        var session = await _sectionFlowService.StartTrainingSessionAsync(request.UserId, sectionId, ct);
        var progress = await _progressRepository.GetAsync(request.UserId, sectionId, ct);
        return Ok(new SectionSessionResponse(
            session.Id,
            sectionId,
            session.Mode,
            session.StartedAt,
            null,
            session.Items.Count,
            MapProgress(progress)));
    }

    [HttpPost("{sectionId:guid}/test/start")]
    public async Task<ActionResult<SectionSessionResponse>> StartTest(Guid sectionId, StartSectionSessionRequest request, CancellationToken ct)
    {
        var session = await _sectionFlowService.StartTestSessionAsync(request.UserId, sectionId, ct);
        var progress = await _progressRepository.GetAsync(request.UserId, sectionId, ct);
        return Ok(new SectionSessionResponse(
            session.Id,
            sectionId,
            session.Mode,
            session.StartedAt,
            progress?.ActiveTestExpiresAt,
            session.Items.Count,
            MapProgress(progress)));
    }

    [HttpPost("{sectionId:guid}/reset")]
    public async Task<ActionResult<ResetSectionAttemptsResponse>> ResetAttempts(Guid sectionId, ResetSectionAttemptsRequest request, CancellationToken ct)
    {
        await _sectionFlowService.ResetTestAttemptsAsync(request.UserId, sectionId, ct);
        var progress = await _progressRepository.GetAsync(request.UserId, sectionId, ct);
        return Ok(new ResetSectionAttemptsResponse(
            sectionId,
            progress?.AttemptCycle ?? 1,
            progress?.AttemptsRemaining ?? SectionProgress.MaxTestAttemptsPerCycle,
            SectionProgress.MaxTestAttemptsPerCycle,
            progress?.HasAttemptsAvailable ?? true));
    }

    [HttpGet("{sectionId:guid}/progress")]
    public async Task<ActionResult<SectionProgressResponse>> GetProgress(Guid sectionId, [FromQuery] Guid userId, CancellationToken ct)
    {
        var progress = await _progressRepository.GetAsync(userId, sectionId, ct);
        return Ok(new SectionProgressResponse(sectionId, MapProgress(progress)));
    }

    private static SectionProgressDto? MapProgress(SectionProgress? progress)
    {
        if (progress is null)
        {
            return null;
        }

        return new SectionProgressDto(
            progress.AttemptCycle,
            progress.AttemptsUsedInCycle,
            progress.AttemptsRemaining,
            SectionProgress.MaxTestAttemptsPerCycle,
            progress.HasAttemptsAvailable,
            progress.ActiveTestSessionId,
            progress.ActiveTestExpiresAt,
            progress.LastTrainingCompletedAt,
            progress.LastTestPassedAt);
    }
}
