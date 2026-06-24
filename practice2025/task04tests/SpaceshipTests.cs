using Xunit;
using task04;

namespace task04tests;

public class SpaceshipTests
{
    [Fact]
    public void Cruiser_ShouldHaveCorrectStats()
    {
        ISpaceship cruiser = new Cruiser();
        Assert.Equal(50, cruiser.Speed);
        Assert.Equal(100, cruiser.FirePower);
    }

    [Fact]
    public void Fighter_ShouldHaveCorrectStats()
    {
        ISpaceship fighter = new Fighter();
        Assert.Equal(100, fighter.Speed);
        Assert.Equal(30, fighter.FirePower);
    }

    [Fact]
    public void Fighter_ShouldBeFasterThanCruiser()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.Speed > cruiser.Speed);
    }

    [Fact]
    public void Cruiser_ShouldHaveMoreFirePowerThanFighter()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(cruiser.FirePower > fighter.FirePower);
    }

    [Fact]
    public void Cruiser_MoveForward_ShouldIncreasePositionBySpeed()
    {
        var cruiser = new Cruiser();
        cruiser.MoveForward();
        Assert.Equal(50, cruiser.Position);
    }

    [Fact]
    public void Fighter_MoveForward_ShouldIncreasePositionBySpeed()
    {
        var fighter = new Fighter();
        fighter.MoveForward();
        Assert.Equal(100, fighter.Position);
    }

    [Fact]
    public void Cruiser_MoveForward_MultipleTimes_AccumulatesPosition()
    {
        var cruiser = new Cruiser();
        cruiser.MoveForward();
        cruiser.MoveForward();
        cruiser.MoveForward();
        Assert.Equal(150, cruiser.Position);
    }

    [Fact]
    public void Rotate_ShouldChangeAngle()
    {
        var cruiser = new Cruiser();
        cruiser.Rotate(90);
        Assert.Equal(90, cruiser.Angle);
    }

    [Fact]
    public void Rotate_ShouldWrapAround360()
    {
        var cruiser = new Cruiser();
        cruiser.Rotate(270);
        cruiser.Rotate(180);
        Assert.Equal(90, cruiser.Angle);
    }

    [Fact]
    public void Fire_ShouldIncrementShotsCount()
    {
        var cruiser = new Cruiser();
        cruiser.Fire();
        cruiser.Fire();
        Assert.Equal(2, cruiser.ShotsCount);
    }

    [Fact]
    public void Fighter_Fire_ShouldIncrementShotsCount()
    {
        var fighter = new Fighter();
        fighter.Fire();
        Assert.Equal(1, fighter.ShotsCount);
    }

    [Fact]
    public void BothShips_ImplementISpaceship()
    {
        ISpaceship cruiser = new Cruiser();
        ISpaceship fighter = new Fighter();

        Assert.NotNull(cruiser);
        Assert.NotNull(fighter);
        Assert.IsAssignableFrom<ISpaceship>(cruiser);
        Assert.IsAssignableFrom<ISpaceship>(fighter);
    }
}
