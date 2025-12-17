using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PhysicsProject.Api.Contracts;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;

namespace PhysicsProject.Api.Controllers;

[ApiController]
[Route("api/study-plan")]
public sealed class StudyPlanController : ControllerBase
{
    private readonly IStudyPlanService _studyPlanService;
    private readonly ISectionProgressRepository _progressRepository;

    public StudyPlanController(IStudyPlanService studyPlanService, ISectionProgressRepository progressRepository)
    {
        _studyPlanService = studyPlanService;
        _progressRepository = progressRepository;
    }

    [HttpGet]
    public async Task<ActionResult<StudyPlanResponse>> GetPlan([FromQuery] Guid? userId, CancellationToken ct)
    {
        var chapters = await _studyPlanService.GetStudyPlanAsync(ct);
        var chapterDtos = new List<StudyPlanChapterDto>(chapters.Count);

        foreach (var chapter in chapters)
        {
            var sections = new List<StudyPlanSectionDto>(chapter.Sections.Count);
            foreach (var section in chapter.Sections.OrderBy(s => s.OrderIndex))
            {
                SectionProgressDto? progressDto = null;
                if (userId is Guid uid)
                {
                    var progress = await _progressRepository.GetAsync(uid, section.Id, ct);
                    progressDto = MapProgress(progress);
                }

                sections.Add(new StudyPlanSectionDto(
                    section.Id,
                    section.ChapterId,
                    section.Title,
                    section.Description,
                    section.OrderIndex,
                    section.DefaultQuestionCount,
                    section.TestTimeLimitSeconds,
                    progressDto));
            }

            chapterDtos.Add(new StudyPlanChapterDto(
                chapter.Id,
                chapter.Title,
                chapter.Description,
                chapter.OrderIndex,
                sections));
        }

        return Ok(new StudyPlanResponse(userId, chapterDtos));
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
