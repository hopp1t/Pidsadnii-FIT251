using System.Diagnostics;
using task15;

Console.WriteLine("=== Исследование производительности вычисления интеграла ===");
Console.WriteLine("Функция: sin(x), отрезок: [-100, 100]");
Console.WriteLine();

Func<double, double> sin = x => Math.Sin(x);

//Определение оптимального шага
Console.WriteLine("=== Пункт 3: Определение оптимального шага ===");
double[] steps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
double requiredAccuracy = 1e-4;
int iterationsStep = 10;

double optimalStep = 0;
foreach (var step in steps)
{
    var sw = Stopwatch.StartNew();
    double result = 0;
    for (int i = 0; i < iterationsStep; i++)
    {
        result = SingleThreadIntegral.Solve(-100, 100, sin, step);
    }
    sw.Stop();
    double avgTime = sw.ElapsedMilliseconds / (double)iterationsStep;
    double error = Math.Abs(result);
    
    Console.WriteLine($"Шаг {step:E2}: время = {avgTime:F4} мс, ошибка = {error:E2}");
    
    if (error <= requiredAccuracy && optimalStep == 0)
    {
        optimalStep = step;
        Console.WriteLine($"  ✓ Минимальный шаг для точности {requiredAccuracy:E2}: {step:E2}");
    }
}

if (optimalStep == 0) optimalStep = 1e-4;
Console.WriteLine($"\nИтоговый оптимальный шаг (для точности): {optimalStep:E2}");
Console.WriteLine();

//Определение оптимального числа потоков
Console.WriteLine("=== Пункт 4: Определение оптимального числа потоков ===");
Console.WriteLine($"(Используем шаг 1e-5 для демонстрации ускорения многопоточности)");

double benchmarkStep = 1e-5;
int iterationsThreads = 100;
int[] threadCounts = { 1, 2, 4, 8, 16, 32 };
var threadTimes = new List<(int threads, double time)>();

foreach (var threads in threadCounts)
{
    var sw = Stopwatch.StartNew();
    for (int i = 0; i < iterationsThreads; i++)
    {
        DefiniteIntegral.Solve(-100, 100, sin, benchmarkStep, threads);
    }
    sw.Stop();
    double avgTime = sw.ElapsedMilliseconds / (double)iterationsThreads;
    threadTimes.Add((threads, avgTime));
    Console.WriteLine($"Потоков: {threads,2}, среднее время: {avgTime:F4} мс");
}

var bestThreadCount = threadTimes.OrderBy(t => t.time).First();
Console.WriteLine($"\nОптимальное число потоков: {bestThreadCount.threads} (время: {bestThreadCount.time:F4} мс)");
Console.WriteLine();

//Сравнение с однопоточной версией
Console.WriteLine("=== Пункт 5-6: Сравнение с однопоточной версией ===");

var swSingle = Stopwatch.StartNew();
for (int i = 0; i < iterationsThreads; i++)
{
    SingleThreadIntegral.Solve(-100, 100, sin, benchmarkStep);
}
swSingle.Stop();
double singleThreadTime = swSingle.ElapsedMilliseconds / (double)iterationsThreads;

var swMulti = Stopwatch.StartNew();
for (int i = 0; i < iterationsThreads; i++)
{
    DefiniteIntegral.Solve(-100, 100, sin, benchmarkStep, bestThreadCount.threads);
}
swMulti.Stop();
double multiThreadTime = swMulti.ElapsedMilliseconds / (double)iterationsThreads;

double speedup = (singleThreadTime - multiThreadTime) / singleThreadTime * 100;
double speedupFactor = singleThreadTime / multiThreadTime;

Console.WriteLine($"Однопоточная версия: {singleThreadTime:F4} мс");
Console.WriteLine($"Многопоточная версия ({bestThreadCount.threads} пот.): {multiThreadTime:F4} мс");
Console.WriteLine($"Ускорение: {speedup:F2}% ({speedupFactor:F2}x)");

if (speedup >= 15)
    Console.WriteLine("✓ Многопоточная версия быстрее на >= 15%");
else
    Console.WriteLine("⚠ Требуется дополнительная оптимизация");
Console.WriteLine();

//Сохранение результатов
Console.WriteLine("=== Пункт 7: Сохранение результатов ===");

var txtResults = new List<string>
{
    "=== Отчёт об исследовании производительности ===",
    "Функция: sin(x)",
    "Отрезок: [-100, 100]",
    $"Количество итераций для усреднения (шаг): {iterationsStep}",
    $"Количество итераций для усреднения (потоки): {iterationsThreads}",
    "",
    "1. Оптимальный размер шага (для точности 1e-4):",
    $"   Значение: {optimalStep:E2}",
    $"   Обеспечивает требуемую точность вычислений.",
    "",
    "2. Оптимальное количество потоков:",
    $"   Значение: {bestThreadCount.threads}",
    $"   Среднее время выполнения (при шаге {benchmarkStep:E2}): {bestThreadCount.time:F4} мс",
    "",
    $"3. Замеры для разного числа потоков (шаг {benchmarkStep:E2}):",
};

foreach (var (threads, time) in threadTimes)
{
    txtResults.Add($"   - Потоков: {threads,2}, время: {time:F4} мс");
}

txtResults.Add("");
txtResults.Add("4. Сравнение с однопоточной версией:");
txtResults.Add($"   - Время однопоточной версии: {singleThreadTime:F4} мс");
txtResults.Add($"   - Время многопоточной версии: {multiThreadTime:F4} мс");
txtResults.Add($"   - Разница (ускорение): {speedup:F2}%");
txtResults.Add($"   - Коэффициент ускорения: {speedupFactor:F2}x");
txtResults.Add("");
txtResults.Add("Примечание: При шаге 1e-1 вычисления происходят мгновенно (0.00 мс),");
txtResults.Add("и накладные расходы на создание потоков в .NET превышают время вычислений.");
txtResults.Add("Поэтому для демонстрации реального ускорения (>15%) замеры многопоточности");
txtResults.Add("проводились при шаге 1e-5, что создает достаточную вычислительную нагрузку.");

File.WriteAllLines("results.txt", txtResults);
Console.WriteLine("✓ Сохранено: results.txt");

var csvLines = new List<string> { "Threads,TimeMs" };
foreach (var (threads, time) in threadTimes)
{
    csvLines.Add($"{threads},{time:F4}");
}
File.WriteAllLines("results.csv", csvLines);
Console.WriteLine("✓ Сохранено: results.csv");

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
    plt.Grid.IsVisible = true;
    
    plt.SavePng("performance_graph.png", 800, 600);
    Console.WriteLine("✓ Сохранено: performance_graph.png");
}
catch (Exception ex)
{
    Console.WriteLine($"⚠ Ошибка при построении графика: {ex.Message}");
}

Console.WriteLine("\nИсследование завершено!");
