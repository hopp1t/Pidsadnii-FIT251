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

//Сравнение с однопоточной версией
Console.WriteLine();
Console.WriteLine("=== Item 5: Comparison with single-thread version ===");
var swSingle = Stopwatch.StartNew();
for (int i = 0; i < iterations; i++)
{
    SingleThreadIntegral.Solve(-100, 100, sin, optimalStep);
}
swSingle.Stop();
double singleThreadTime = swSingle.ElapsedMilliseconds / (double)iterations;

var swMulti = Stopwatch.StartNew();
for (int i = 0; i < iterations; i++)
{
    DefiniteIntegral.Solve(-100, 100, sin, optimalStep, bestThreadCount.threads);
}
swMulti.Stop();
double multiThreadTime = swMulti.ElapsedMilliseconds / (double)iterations;

double speedup = (singleThreadTime - multiThreadTime) / singleThreadTime * 100;

Console.WriteLine($"Single-thread time: {singleThreadTime:F2} ms");
Console.WriteLine($"Multi-thread time ({bestThreadCount.threads} threads): {multiThreadTime:F2} ms");
Console.WriteLine($"Speedup: {speedup:F2}%");

if (speedup >= 15)
{
    Console.WriteLine("✓ Multi-thread version is >= 15% faster");
}
else
{
    Console.WriteLine("⚠ Optimization required");
}

//Финальные замеры после оптимизации
Console.WriteLine();
Console.WriteLine("=== Item 6: Measurements after optimization ===");

var swSingleFinal = Stopwatch.StartNew();
for (int i = 0; i < iterations; i++)
{
    SingleThreadIntegral.Solve(-100, 100, sin, optimalStep);
}
swSingleFinal.Stop();
double singleThreadTimeFinal = swSingleFinal.ElapsedMilliseconds / (double)iterations;

var swMultiFinal = Stopwatch.StartNew();
for (int i = 0; i < iterations; i++)
{
    DefiniteIntegral.Solve(-100, 100, sin, optimalStep, bestThreadCount.threads);
}
swMultiFinal.Stop();
double multiThreadTimeFinal = swMultiFinal.ElapsedMilliseconds / (double)iterations;

double speedupFinal = (singleThreadTimeFinal - multiThreadTimeFinal) / singleThreadTimeFinal * 100;

Console.WriteLine($"Single-thread time: {singleThreadTimeFinal:F2} ms");
Console.WriteLine($"Multi-thread time: {multiThreadTimeFinal:F2} ms");
Console.WriteLine($"Speedup: {speedupFinal:F2}%");
