using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Services;

public sealed class TemplateService : ITemplateService
{
    private readonly IProblemRepository _problems;
    private readonly IEnumerable<ITaskGenerator> _generators;

    public TemplateService(IProblemRepository problems, IEnumerable<ITaskGenerator> generators)
    {
        _problems = problems;
        _generators = generators;
    }

    public async Task<ProblemInstance> GenerateInstanceAsync(Guid templateId, long? seed, GenerationConstraints constraints, CancellationToken ct)
    {
        var template = await _problems.GetTemplateByIdAsync(templateId, ct)
            ?? throw new InvalidOperationException("Template not found");
        var generator = _generators.FirstOrDefault(g => g.CanHandle(template.TemplateType))
            ?? throw new InvalidOperationException($"No generator for template type {template.TemplateType}");

        var effectiveSeed = seed ?? DeriveSeed(template.Id);
        var instance = generator.Generate(template, effectiveSeed, constraints);
        instance.AssignTemplateInfo(template.TemplateType);
        await _problems.SaveInstanceAsync(instance, ct);
        return instance;
    }

    private static long DeriveSeed(Guid templateId)
    {
        // Combine templateId bits with current timestamp for reproducible-ish per-time generation
        var bytes = templateId.ToByteArray();
        long guidPart = BitConverter.ToInt64(bytes, 0);
        long ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return guidPart ^ ts;
    }
}


