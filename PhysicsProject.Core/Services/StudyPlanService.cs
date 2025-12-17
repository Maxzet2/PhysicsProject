using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Services;

public sealed class StudyPlanService : IStudyPlanService
{
    private readonly IChapterRepository _chapterRepository;

    public StudyPlanService(IChapterRepository chapterRepository)
    {
        _chapterRepository = chapterRepository;
    }

    public Task<IReadOnlyList<Chapter>> GetStudyPlanAsync(CancellationToken ct)
        => _chapterRepository.GetAllAsync(ct);
}
