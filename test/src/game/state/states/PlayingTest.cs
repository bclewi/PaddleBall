namespace PaddleBall.Tests;

using System.Linq;
using Chickensoft.Collections;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using FluentAssertions;
using Godot;
using Moq;

public class PlayingTest(Node testScene) : TestClass(testScene)
{
    private AutoProp<bool> _isMouseCaptured = default!;
    private AutoProp<bool> _isPaused = default!;

    private Mock<IGameRepo> _gameRepo = default!;

    private IFakeContext _context = default!;

    private GameLogic.State.Playing _state = default!;

    [Setup]
    public void Setup()
    {
        _state = new GameLogic.State.Playing();
        _context = _state.CreateFakeContext();

        _isMouseCaptured = new(true);
        _isPaused = new(true);

        _gameRepo = new();
        _gameRepo.Setup(x => x.IsPaused).Returns(_isPaused);
        _gameRepo.Setup(x => x.IsMouseCaptured).Returns(_isMouseCaptured);

        _context.Set(_gameRepo.Object);
    }

    [Test]
    public void OnEnter()
    {
        _gameRepo.Reset();
        _gameRepo.Setup(x => x.SetIsMouseCaptured(true));
        _state.Enter();
        _context.Outputs.Single().Should().BeOfType<GameLogic.Output.StartGame>();
        _gameRepo.VerifyAll();
    }

    [Test]
    public void Subscribes()
    {
        _state.Attach(_context);

        _gameRepo.VerifyAdd(x => x.OnGameEnded += _state.OnEnded);

        _state.Detach();

        _gameRepo.VerifyRemove(x => x.OnGameEnded -= _state.OnEnded);
    }

    [Test]
    public void OnPauseButtonPressed() =>
      _state.On(new GameLogic.Input.PauseButtonPressed()).State
        .Should().BeOfType<GameLogic.State.Paused>();

    [Test]
    public void OnEnded()
    {
        _state.OnEnded(GameOverReason.Won);
        _context.Inputs.Single().Should().BeOfType<GameLogic.Input.EndGame>();
    }

    [Test]
    public void OnEndGameWins()
    {
        var result = _state.On(new GameLogic.Input.EndGame(GameOverReason.Won));
        _gameRepo.Verify(x => x.Pause());
        result.State.Should().BeOfType<GameLogic.State.Won>();
    }

    [Test]
    public void OnEndGameQuits()
    {
        var result = _state.On(new GameLogic.Input.EndGame(GameOverReason.Quit));
        _gameRepo.Verify(x => x.Pause());
        result.State.Should().BeOfType<GameLogic.State.Quit>();
    }
}
