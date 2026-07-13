using System.Diagnostics;
using task15;

Console.WriteLine("=== Исследование производительности вычисления интеграла ===");
Console.WriteLine("Функция: sin(x), отрезок: [-100, 100]");
Console.WriteLine();

Func<double, double> sin = x => Math.Sin(x);
int iterations = 10; // Количество замеров для усреднения

//Определение оптимального шага
Console.WriteLine("=== Пункт 3: Определение оптимального шага ===");
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
    double error = Math.Abs(result); // Теоретический интеграл sin(x) на [-100, 100] = 0
    
    Console.WriteLine($"Шаг {step:E2}: время = {avgTime:F2} мс, ошибка = {error:E2}");
    
    if (error <= requiredAccuracy && optimalStep == 0)
    {
        optimalStep = step;
        Console.WriteLine($"  ✓ Минимальный шаг для точности {requiredAccuracy:E2}: {step:E2}");
    }
}

if (optimalStep == 0) optimalStep = 1e-4;
Console.WriteLine($"\nИтоговый оптимальный шаг: {optimalStep:E2}");
Console.WriteLine();

// Определение оптимального числа потоков
Console.WriteLine("=== Пункт 4: Определение оптимального числа потоков ===");
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
    Console.WriteLine($"Потоков: {threads,2}, время: {avgTime:F2} мс");
}

var bestThreadCount = threadTimes.OrderBy(t => t.time).First();
Console.WriteLine($"\nОптимальное число потоков: {bestThreadCount.threads} (время: {bestThreadCount.time:F2} мс)");
Console.WriteLine();

//Сравнение и оптимизация
Console.WriteLine("=== Пункт 5-6: Сравнение с однопоточной версией ===");
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
double speedupFactor = singleThreadTime / multiThreadTime;

Console.WriteLine($"Однопоточная версия: {singleThreadTime:F2} мс");
Console.WriteLine($"Многопоточная версия ({bestThreadCount.threads} пот.): {multiThreadTime:F2} мс");
Console.WriteLine($"Ускорение: {speedup:F2}% ({speedupFactor:F2}x)");

if (speedup >= 15)
    Console.WriteLine("✓ Многопоточная версия быстрее на >= 15%");
else
    Console.WriteLine("⚠ Требуется дополнительная оптимизация");
Console.WriteLine();

//Сохранение результатов
Console.WriteLine("=== Пункт 7: Сохранение результатов ===");

//TXT отчёт
var txtResults = new List<string>
{
    "=== Отчёт об исследовании производительности ===",
    "Функция: sin(x)",
    "Отрезок: [-100, 100]",
    $"Количество итераций для усреднения: {iterations}",
    "",
    "1. Оптимальный размер шага:",
    $"   Значение: {optimalStep:E2}",
    $"   Обеспечивает точность вычислений: {requiredAccuracy:E2}",
    "",
    "2. Оптимальное количество потоков:",
    $"   Значение: {bestThreadCount.threads}",
    $"   Среднее время выполнения: {bestThreadCount.time:F2} мс",
    "",
    "3. Замеры для разного числа потоков:",
};
foreach (var (threads, time) in threadTimes)
{
    txtResults.Add($"   - Потоков: {threads,2}, время: {time:F2} мс");
}

txtResults.Add("");
txtResults.Add("4. Сравнение с однопоточной версией:");
txtResults.Add($"   - Время однопоточной версии: {singleThreadTime:F2} мс");
txtResults.Add($"   - Время многопоточной версии: {multiThreadTime:F2} мс");
txtResults.Add($"   - Разница (ускорение): {speedup:F2}%");
txtResults.Add($"   - Коэффициент ускорения: {speedupFactor:F2}x");

File.WriteAllLines("results.txt", txtResults);
Console.WriteLine("✓ Сохранено: results.txt");

//CSV для графиков/Excel
var csvLines = new List<string> { "Threads,TimeMs" };
foreach (var (threads, time) in threadTimes)
{
    csvLines.Add($"{threads},{time:F4}");
}
File.WriteAllLines("results.csv", csvLines);
Console.WriteLine("✓ Сохранено: results.csv");

//PNG график через ScottPlot
try
{
    var plt = new ScottPlot.Plot();
    var xs = threadTimes.Select(t => (double)t.threads).ToArray();
    var ys = threadTimes.Select(t => t.time).ToArray();
    
    var scatter = plt.Add.Scatter(xs, ys);
    scatter.MarkerSize = 8;
    scatter.LineWidth = 2;
    
    plt.Title("Зависимость времени вычисления интеграла от числа потоков");
    plt.XLabel("Количество потоков");
    plt.YLabel("Время выполнения, мс");
    
    plt.Grid.Enable();
    
    plt.SavePng("performance_graph.png", 800, 600);
    Console.WriteLine("✓ Сохранено: performance_graph.png");
}
catch (Exception ex)
{
    Console.WriteLine($"⚠ Ошибка при построении графика: {ex.Message}");
}

Console.WriteLine("\nИсследование завершено!");
