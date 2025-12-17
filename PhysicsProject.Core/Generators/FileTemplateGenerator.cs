using System.Data;
using System.Text.Json;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Generators;

// TemplateType convention: "file.v1". ProblemTemplate.JsonSpec should contain absolute or project-relative path to a JSON file.
public sealed class FileTemplateGenerator : ITaskGenerator
{
    public bool CanHandle(string templateType) => templateType == "file.v1";

    public ProblemInstance Generate(ProblemTemplate template, long seed, GenerationConstraints constraints)
    {
        var path = template.JsonSpec;
        if (string.IsNullOrWhiteSpace(path))
            throw new InvalidOperationException("JsonSpec must be a file path for file.v1 templates");

        // Resolve relative paths against application base directory to work in tests and runtime
        if (!Path.IsPathRooted(path))
        {
            var baseDir = AppContext.BaseDirectory;
            path = Path.GetFullPath(Path.Combine(baseDir, path));
        }
        if (!File.Exists(path))
            throw new FileNotFoundException($"Template file not found: {path}");

        var json = File.ReadAllText(path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        FileTemplateSpec spec;
        if (json.TrimStart().StartsWith("["))
        {
            var list = JsonSerializer.Deserialize<List<FileTemplateSpec>>(json, options) ?? throw new InvalidOperationException("Invalid file template spec array");
            if (list.Count == 0) throw new InvalidOperationException("Empty template list");
            var index = (int)(Math.Abs(seed) % list.Count);
            spec = list[index];
        }
        else
        {
            spec = JsonSerializer.Deserialize<FileTemplateSpec>(json, options) ?? throw new InvalidOperationException("Invalid file template spec");
        }

        // Very simple placeholder substitution using provided parameters
        var parameters = new Dictionary<string, object>(spec.Parameters ?? new Dictionary<string, object>());
        var statement = spec.Statement ?? string.Empty;
        foreach (var kv in parameters)
        {
            var valueString = ConvertParamToString(kv.Value);
            statement = statement.Replace("{" + kv.Key + "}", valueString);
        }

        var answer = spec.Answer ?? ComputeAnswer(spec.AnswerExpression, parameters);
        var instance = new ProblemInstance
        {
            Id = Guid.NewGuid(),
            TemplateId = template.Id,
            Seed = seed,
            Statement = statement,
            Parameters = parameters,
            NormalizedCorrectAnswer = answer
        };
        instance.AssignTemplateInfo(template.TemplateType);
        return instance;
    }

    private sealed class FileTemplateSpec
    {
        public string? Statement { get; set; }
        public Dictionary<string, object>? Parameters { get; set; }
        public string? Answer { get; set; }
        public string? AnswerExpression { get; set; }
    }

    private static string ConvertParamToString(object? value)
    {
        if (value is null) return string.Empty;
        if (value is JsonElement elem)
        {
            return elem.ValueKind switch
            {
                JsonValueKind.String => elem.GetString() ?? string.Empty,
                JsonValueKind.Number => elem.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => elem.GetRawText()
            };
        }
        return value.ToString() ?? string.Empty;
    }

    private static string ComputeAnswer(string? expression, IDictionary<string, object> parameters)
    {
        if (string.IsNullOrWhiteSpace(expression)) return string.Empty;
        // Replace placeholders with numeric literals
        var expr = expression;
        foreach (var kv in parameters)
        {
            var valueString = ConvertParamToString(kv.Value);
            expr = expr.Replace("{" + kv.Key + "}", valueString);
            expr = expr.Replace(kv.Key, valueString);
        }
        // Evaluate using DataTable.Compute for basic arithmetic
        var table = new DataTable();
        var result = table.Compute(expr, null);
        return Convert.ToString(result) ?? string.Empty;
    }
}


