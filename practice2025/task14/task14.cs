using System.Threading;

namespace task14;

public static class DefiniteIntegral
{
    private static readonly object _lock = new();

    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        ArgumentNullException.ThrowIfNull(function);
        if (step <= 0)
            throw new ArgumentOutOfRangeException(nameof(step), "Шаг должен быть положительным");
        if (threadsNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(threadsNumber), "Число потоков должно быть положительным");

        double totalResult = 0.0;
        var barrier = new Barrier(threadsNumber + 1);
        double segmentWidth = (b - a) / threadsNumber;
        var threads = new Thread[threadsNumber];

        for (int i = 0; i < threadsNumber; i++)
        {
            double segmentA = a + i * segmentWidth;
            double segmentB = a + (i + 1) * segmentWidth;

            threads[i] = new Thread(() =>
            {
                double localResult = ComputeTrapezoidal(segmentA, segmentB, function, step);
                
                lock (_lock)
                {
                    totalResult += localResult;
                }
                
                barrier.SignalAndWait();
            });

            threads[i].Start();
        }

        barrier.SignalAndWait();

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return totalResult;
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
