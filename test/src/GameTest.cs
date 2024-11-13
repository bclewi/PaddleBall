namespace PaddleBall.Tests;

using System.Threading.Tasks;
using Chickensoft.GoDotTest;
using Chickensoft.GodotTestDriver;
using Chickensoft.GodotTestDriver.Drivers;
using FluentAssertions;
using Godot;

public class GameTest(Node testScene) : TestClass(testScene)
{
    private Game game = default!;
    private Fixture fixture = default!;

    [SetupAll]
    public async Task Setup()
    {
        fixture = new Fixture(TestScene.GetTree());
        game = await fixture.LoadAndAddScene<Game>();
    }

    [CleanupAll]
    public void Cleanup() => fixture.Cleanup();

    [Test]
    public void TestButtonUpdatesCounter()
    {
        var buttonDriver = new ButtonDriver(() => game.TestButton);
        buttonDriver.ClickCenter();
        game.ButtonPresses.Should().Be(1);
    }
}
