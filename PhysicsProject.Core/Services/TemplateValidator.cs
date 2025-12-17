using System.Text.Json;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Services;

public sealed class TemplateValidator : ITemplateValidator
{
    public ValidationResult Validate(ProblemTemplate template)
    {
        if (template is null) return new ValidationResult(false, "Template is null");
        if (string.IsNullOrWhiteSpace(template.TemplateType)) return new ValidationResult(false, "TemplateType is required");
        if (template.TemplateType == "file.v1")
        {
            var path = template.JsonSpec;
            if (string.IsNullOrWhiteSpace(path)) return new ValidationResult(false, "JsonSpec path is required for file.v1");
            if (!Path.IsPathRooted(path))
            {
                var baseDir = AppContext.BaseDirectory;
                path = Path.GetFullPath(Path.Combine(baseDir, path));
            }
            if (!File.Exists(path)) return new ValidationResult(false, $"File not found: {path}");
            try
            {
                var json = File.ReadAllText(path);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                if (json.TrimStart().StartsWith("["))
                {
                    var list = JsonSerializer.Deserialize<List<FileSpec>>(json, options) ?? new List<FileSpec>();
                    if (list.Count == 0) return new ValidationResult(false, "Template array is empty");
                    if (!list.Any(IsSpecValid)) return new ValidationResult(false, "No valid entries in template array");
                }
                else
                {
                    var spec = JsonSerializer.Deserialize<FileSpec>(json, options);
                    if (spec is null || !IsSpecValid(spec)) return new ValidationResult(false, "Invalid template spec");
                }
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, ex.Message);
            }
        }
        return new ValidationResult(true, "OK");
    }

    private static bool IsSpecValid(FileSpec spec)
    {
        if (string.IsNullOrWhiteSpace(spec.Statement)) return false;
        // parameters optional, but if present should be object
        return true;
    }

    private sealed class FileSpec
    {
        public string? Statement { get; set; }
        public Dictionary<string, object>? Parameters { get; set; }
        public string? Answer { get; set; }
        public string? AnswerExpression { get; set; }
    }
}





