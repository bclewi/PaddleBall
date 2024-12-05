namespace PaddleBall.Tests;

using System.Linq;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using FluentAssertions;
using Godot;
using Moq;

public class ResumingTest(Node testScene) : TestClass(testScene)
{
    private Mock<IGameRepo> _gameRepo = default!;

    private IFakeContext _context = default!;

    private GameLogic.State.Resuming _state = default!;

    [Setup]
    public void Setup()
    {
        _state = new GameLogic.State.Resuming();
        _context = _state.CreateFakeContext();

        _gameRepo = new();
        _context.Set(_gameRepo.Object);
    }

    [Test]
    public void OnEnter()
    {
        _gameRepo.Setup(x => x.Resume());
        _state.Enter();
        _gameRepo.VerifyAll();
    }

    [Test]
    public void OnExit()
    {
        _state.Exit();
        _context.Outputs.Single().Should().BeOfType<GameLogic.Output.HidePauseMenu>();
    }

    [Test]
    public void OnPauseMenuTransitioned()
    {
        var result = _state.On(new GameLogic.Input.PauseMenuTransitioned());
        result.State.Should().BeOfType<GameLogic.State.Playing>();
    }
}
