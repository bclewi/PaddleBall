namespace PaddleBall.Tests;

using System.Linq;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using FluentAssertions;
using Godot;
using Moq;

public class PausedTest(Node testScene) : TestClass(testScene)
{
    private Mock<IGameRepo> _gameRepo = default!;

    private IFakeContext _context = default!;

    private GameLogic.State.Paused _state = default!;

    [Setup]
    public void Setup()
    {
        _state = new GameLogic.State.Paused();
        _context = _state.CreateFakeContext();

        _gameRepo = new();
        _context.Set(_gameRepo.Object);
    }

    [Test]
    public void OnEnter()
    {
        _gameRepo.Setup(x => x.Pause());
        _state.Enter();
        _context.Outputs.Single().Should().BeOfType<GameLogic.Output.ShowPauseMenu>();
        _gameRepo.VerifyAll();
    }

    [Test]
    public void OnExit()
    {
        _state.Exit();
        _context.Outputs.Single().Should().BeOfType<GameLogic.Output.ExitPauseMenu>();
    }

    [Test]
    public void OnPauseButtonPressed()
    {
        var result = _state.On(new GameLogic.Input.PauseButtonPressed());
        result.State.Should().BeOfType<GameLogic.State.Resuming>();
    }

    [Test]
    public void OnGoToMainMenu()
    {
        var result = _state.On(new GameLogic.Input.GoToMainMenu());
        result.State.Should().BeOfType<GameLogic.State.Quit>();
    }
}
