using System.Diagnostics;
using task13;

Console.WriteLine("=== Performance Research for Integral Computation ===");
Console.WriteLine("Function: sin(x), Interval: [-100, 100]");
Console.WriteLine();

Func<double, double> sin = x => Math.Sin(x);
int iterations = 10;

//Определение оптимального шага
Console.WriteLine("=== Item 3: Optimal step size ===");
double[] steps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
double requiredAccuracy = 1e-4;

double optimalStep = 0;
foreach (var step in steps)
{
    var sw = Stopwatch.StartNew();
    double result = 0;
    for (int i = 0; i < iterations; i++)
    {
        result = SingleThreadIntegral.Solve(-100, 100, sin, step);
    }
    sw.Stop();
    double avgTime = sw.ElapsedMilliseconds / (double)iterations;
    double error = Math.Abs(result);
    
    Console.WriteLine($"Step {step:E2}: time = {avgTime:F2} ms, error = {error:E2}");
    
    if (error <= requiredAccuracy && optimalStep == 0)
    {
        optimalStep = step;
        Console.WriteLine($"  ✓ Minimal step for accuracy {requiredAccuracy:E2}: {step:E2}");
    }
}

if (optimalStep == 0) optimalStep = 1e-4;
Console.WriteLine($"\nOptimal step: {optimalStep:E2}");
