namespace PaddleBall.Tests;

using Chickensoft.GoDotTest;
using FluentAssertions;
using Godot;

public class GameLogicTest(Node testScene) : TestClass(testScene)
{
    private GameLogic _logic = default!;

    [Setup]
    public void Setup() => _logic = new GameLogic();

    [Test]
    public void Initializes() => _logic.GetInitialState().State
        .Should().BeAssignableTo<GameLogic.State>();
}
