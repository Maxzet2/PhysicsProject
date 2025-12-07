using System.Globalization;
using PhysicsProject.Core.Abstractions;
using PhysicsProject.Core.Domain;

namespace PhysicsProject.Core.Services;

public sealed class SimpleAnswerEvaluator : IAnswerEvaluator
{
    public bool CanHandle(string templateType) => true; // fallback evaluator

    public EvaluationResult Evaluate(ProblemInstance instance, UserAnswer answer)
    {
        var correct = instance.NormalizedCorrectAnswer?.Trim() ?? string.Empty;
        var raw = answer.Raw?.Trim() ?? string.Empty;

        if (TryParseNumber(correct, out var c) && TryParseNumber(raw, out var a))
        {
            var isOk = Math.Abs(c - a) <= 1e-6 * Math.Max(1.0, Math.Abs(c));
            return new EvaluationResult(isOk, isOk ? 1m : 0m, isOk ? "OK" : $"Expected {c}, got {a}");
        }

        var eq = string.Equals(correct, raw, StringComparison.OrdinalIgnoreCase);
        return new EvaluationResult(eq, eq ? 1m : 0m, eq ? "OK" : $"Expected '{correct}', got '{raw}'");
    }

    private static bool TryParseNumber(string s, out double value)
    {
        return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
               || double.TryParse(s, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
    }
}





