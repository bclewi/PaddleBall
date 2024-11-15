namespace PaddleBall.Tests;

using Chickensoft.GoDotTest;
using FluentAssertions;
using Godot;

public class InGameUILogicTest(Node testScene) : TestClass(testScene)
{
    private InGameUILogic logic = default!;

    [Setup]
    public void Setup() => logic = new InGameUILogic();

    [Test]
    public void Initializes() => logic.GetInitialState().State
        .Should().BeAssignableTo<InGameUILogic.State>();
}
