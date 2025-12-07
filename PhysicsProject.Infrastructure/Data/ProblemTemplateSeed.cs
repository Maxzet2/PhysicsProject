using PhysicsProject.Core.Domain;

namespace PhysicsProject.Infrastructure.Data;

public static class ProblemTemplateSeed
{
    public static readonly Guid DefaultTemplateId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static IEnumerable<ProblemTemplate> LoadDefaultTemplates(string baseDirectory)
    {
        var relativePath = Path.Combine("Data", "default_template.json");
        var absolutePath = Path.Combine(baseDirectory, relativePath);
        if (!File.Exists(absolutePath))
        {
            absolutePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relativePath));
        }

        return new[]
        {
            new ProblemTemplate
            {
                Id = DefaultTemplateId,
                Name = "Базовый физический тест",
                TemplateType = "file.v1",
                JsonSpec = relativePath
            }
        };
    }
}

