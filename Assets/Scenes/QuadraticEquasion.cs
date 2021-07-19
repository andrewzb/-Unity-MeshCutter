using System;

public enum ExecutionResult
{
    Success,
    Fail,
}

public class QuadraticEquasion
{
    public ExecutionResult Solve(float a, float b, float c, out Tuple<float, float> result)
    {
        var discriminant = b * b - 4 * a * c;
        if (discriminant < 0)
        {
            result = null;
            return ExecutionResult.Fail;
        }
        else
            return SolverSimple(a, b, c, discriminant, out result);
    }

    private ExecutionResult SolverSimple(float a, float b, float c, float discriminant, out Tuple<float, float> result)
    {
        var rootDisc = Math.Sqrt(discriminant);
        result = Tuple.Create(
            (float)(-b + rootDisc) / (2 * a),
            (float)(-b - rootDisc) / (2 * a)
            );
        return ExecutionResult.Success;
    }
}
