namespace task04;

public class Fighter : ISpaceship
{
    private int _position;
    private int _angle;
    private readonly List<string> _shots = new();

    public int Speed { get; } = 100;
    public int FirePower { get; } = 30;

    public void MoveForward() => _position += Speed;

    public void Rotate(int angle) => _angle = (_angle + angle) % 360;

    public void Fire() => _shots.Add($"Fighter fired with power {FirePower}");

    /// <summary>Текущая позиция корабля (для тестов).</summary>
    public int Position => _position;

    /// <summary>Текущий угол поворота (для тестов).</summary>
    public int Angle => _angle;

    /// <summary>Количество произведённых выстрелов (для тестов).</summary>
    public int ShotsCount => _shots.Count;
}