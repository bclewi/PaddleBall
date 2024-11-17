namespace PaddleBall;

using Chickensoft.GoDotTest;
using FluentAssertions;
using Godot;

public class AppLogicTest(Node testScene) : TestClass(testScene)
{
    private AppLogic logic = default!;

    [Setup]
    public void Setup() => logic = new();

    [Test]
    public void Initializes() => logic.GetInitialState().State
        .Should().BeAssignableTo<AppLogic.State>();
}
