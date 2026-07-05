namespace task11;

public static class CalculatorGenerator
{
    public static ICalculator CreateCalculator()
    {
        var type = Type.GetType("Calculator");
        if (type == null)
            throw new InvalidOperationException("Class Calculator not found");
        
        return (ICalculator)Activator.CreateInstance(type)!;
    }
}
