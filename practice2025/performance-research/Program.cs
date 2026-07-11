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

//Определение оптимального числа потоков
Console.WriteLine();
Console.WriteLine("=== Item 4: Optimal thread count ===");
int[] threadCounts = { 1, 2, 4, 8, 16, 32 };
var threadTimes = new List<(int threads, double time)>();

foreach (var threads in threadCounts)
{
    var sw = Stopwatch.StartNew();
    for (int i = 0; i < iterations; i++)
    {
        DefiniteIntegral.Solve(-100, 100, sin, optimalStep, threads);
    }
    sw.Stop();
    double avgTime = sw.ElapsedMilliseconds / (double)iterations;
    threadTimes.Add((threads, avgTime));
    Console.WriteLine($"Threads: {threads,2}, time: {avgTime:F2} ms");
}

var bestThreadCount = threadTimes.OrderBy(t => t.time).First();
Console.WriteLine($"\nOptimal thread count: {bestThreadCount.threads} (time: {bestThreadCount.time:F2} ms)");

// Построение графика
Console.WriteLine();
Console.WriteLine("=== Building graph ===");
try
{
    var plt = new ScottPlot.Plot();
    var xs = threadTimes.Select(t => (double)t.threads).ToArray();
    var ys = threadTimes.Select(t => t.time).ToArray();
    plt.Add.Scatter(xs, ys);
    plt.Title("Execution time vs Thread count");
    plt.XLabel("Thread count");
    plt.YLabel("Execution time, ms");
    plt.SavePng("performance_graph.png", 800, 600);
    Console.WriteLine("Graph saved to performance_graph.png");
}
catch (Exception ex)
{
    Console.WriteLine($"Error building graph: {ex.Message}");
}
