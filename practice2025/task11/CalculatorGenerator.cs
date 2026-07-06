using System.Reflection;
using System.Reflection.Emit;

namespace task11;

public static class CalculatorGenerator
{
    public static ICalculator CreateCalculator()
    {
        var assemblyName = new AssemblyName("DynamicCalculatorAssembly");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            assemblyName, AssemblyBuilderAccess.Run);

        var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicCalculatorModule");

        var typeBuilder = moduleBuilder.DefineType(
            "DynamicCalculator",
            TypeAttributes.Public | TypeAttributes.Class,
            null,
            new[] { typeof(ICalculator) });

        GenerateMethod(typeBuilder, nameof(ICalculator.Add), OpCodes.Add);
        GenerateMethod(typeBuilder, nameof(ICalculator.Minus), OpCodes.Sub);
        GenerateMethod(typeBuilder, nameof(ICalculator.Mul), OpCodes.Mul);
        GenerateMethod(typeBuilder, nameof(ICalculator.Div), OpCodes.Div);

        var calculatorType = typeBuilder.CreateType();
        return (ICalculator)Activator.CreateInstance(calculatorType)!;
    }

    private static void GenerateMethod(TypeBuilder typeBuilder, string methodName, OpCode operation)
    {
        var methodBuilder = typeBuilder.DefineMethod(
            methodName,
            MethodAttributes.Public | MethodAttributes.Virtual,
            CallingConventions.Standard,
            typeof(int),
            new[] { typeof(int), typeof(int) });

        var il = methodBuilder.GetILGenerator();
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ldarg_2);
        il.Emit(operation);
        il.Emit(OpCodes.Ret);

        var interfaceMethod = typeof(ICalculator).GetMethod(methodName)!;
        typeBuilder.DefineMethodOverride(methodBuilder, interfaceMethod);
    }
}
    