namespace task07;

[DisplayName("Пример класса")]
[Version(1, 0)]
public class SampleClass
{
    [DisplayName("Числовое свойство")]
    public int Number { get; set; }

    public string Name { get; set; } = string.Empty;

    [DisplayName("Тестовый метод")]
    public void TestMethod() { }

    public void AnotherMethod() { }
}
