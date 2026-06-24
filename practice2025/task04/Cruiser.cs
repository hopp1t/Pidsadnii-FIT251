namespace task04;

public class Cruiser : ISpaceship
{
    private int _position;
    private int _angle;
    private readonly List<string> _shots = new();

    public int Speed { get; } = 50;
    public int FirePower { get; } = 100;

    public void MoveForward() => _position += Speed;

    public void Rotate(int angle) => _angle = (_angle + angle) % 360;

    public void Fire() => _shots.Add($"Cruiser fired with power {FirePower}");

    public int Position => _position;

    public int Angle => _angle;

    public int ShotsCount => _shots.Count;
}
