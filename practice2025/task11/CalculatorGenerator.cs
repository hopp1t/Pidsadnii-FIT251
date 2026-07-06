using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace task11;

public static class CalculatorGenerator
{
    private const string CalculatorCode = @"
using System;

public class Calculator : task11.ICalculator
{
    public int Add(int a, int b) => a + b;
    public int Minus(int a, int b) => a - b;
    public int Mul(int a, int b) => a * b;
    public int Div(int a, int b) => a / b;
}";

    private static readonly Lazy<ICalculator> _instance = new(CreateCalculatorInternal);

    public static ICalculator CreateCalculator() => _instance.Value;

    public static ICalculator CreateCalculatorFromCode(string code)
    {
        return CompileAndCreateInstance(code);
    }

    private static ICalculator CreateCalculatorInternal()
    {
        return CompileAndCreateInstance(CalculatorCode);
    }

    private static ICalculator CompileAndCreateInstance(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);

        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
        };

        var compilation = CSharpCompilation.Create(
            "CalculatorAssembly",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        EmitResult result = compilation.Emit(ms);

        if (!result.Success)
        {
            var errors = result.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => d.GetMessage());
            
            throw new InvalidOperationException(
                $"Ошибка компиляции:\n{string.Join("\n", errors)}");
        }

        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());

        var calculatorType = assembly.GetType("Calculator")
            ?? throw new InvalidOperationException("Класс Calculator не найден в скомпилированной сборке");

        var instance = Activator.CreateInstance(calculatorType) as ICalculator
            ?? throw new InvalidOperationException("Не удалось создать экземпляр класса Calculator");

        return instance;
    }
}
