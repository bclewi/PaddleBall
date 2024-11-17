namespace PaddleBall;

using System;
using System.Linq;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using FluentAssertions;
using Godot;
using Moq;

public class MainMenuTest(Node testScene) : TestClass(testScene)
{
    private Mock<IAppRepo> _appRepo = default!;
    private IFakeContext _context = default!;

    private AppLogic.State.MainMenu state = default!;

    [Setup]
    public void Setup()
    {
        state = new();
        _appRepo = new();
        _context = state.CreateFakeContext();
        _context.Set(_appRepo.Object);
    }

    [Test]
    public void Enters()
    {
        state.Enter();

        var expectedOutputs = new object[]
        {
            new AppLogic.Output.SetupGameScene(),
            new AppLogic.Output.ShowMainMenu()
        };

        _context.Outputs.Should().ContainInConsecutiveOrder(expectedOutputs);
    }

    [Test]
    public void StartsGame()
    {
        var next = state.On(new AppLogic.Input.NewGame());

        next.State.Should().BeOfType<AppLogic.State.LeavingMenu>();
    }
}
