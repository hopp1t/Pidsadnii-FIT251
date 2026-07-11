namespace task13;

/// Однопоточная реализация вычисления определённого интеграла методом трапеций.
public static class SingleThreadIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step)
    {
        ArgumentNullException.ThrowIfNull(function);
        if (step <= 0)
            throw new ArgumentOutOfRangeException(nameof(step), "Шаг должен быть положительным");

        return ComputeTrapezoidal(a, b, function, step);
    }

    private static double ComputeTrapezoidal(double a, double b, Func<double, double> function, double step)
    {
        int steps = (int)Math.Round((b - a) / step);
        if (steps <= 0) return 0.0;

        double actualStep = (b - a) / steps;
        double result = function(a) / 2.0 + function(b) / 2.0;

        for (int i = 1; i < steps; i++)
        {
            result += function(a + i * actualStep);
        }

        return result * actualStep;
    }
}
