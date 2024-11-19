namespace PaddleBall.Tests;

using System.Linq;
using Chickensoft.Collections;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using FluentAssertions;
using Godot;
using Moq;

public class MenuBackdropTest(Node testScene) : TestClass(testScene)
{
    private Mock<IAppRepo> _appRepo = default!;
    private Mock<IGameRepo> _gameRepo = default!;

    private IFakeContext _context = default!;

    private GameLogic.State.MenuBackdrop _state = default!;

    [Setup]
    public void Setup()
    {
        _appRepo = new();
        _gameRepo = new();

        _state = new GameLogic.State.MenuBackdrop();

        _context = _state.CreateFakeContext();
        _context.Set(_appRepo.Object);
        _context.Set(_gameRepo.Object);

        _gameRepo.Setup(x => x.IsMouseCaptured).Returns(new Mock<IAutoProp<bool>>().Object);
        _gameRepo.Setup(x => x.IsPaused).Returns(new Mock<IAutoProp<bool>>().Object);
    }

    [Test]
    public void Subscribes()
    {
        _state.Attach(_context);

        _appRepo.VerifyAdd(x => x.GameEntered += _state.OnGameEntered);

        _state.Detach();

        _appRepo.VerifyRemove(x => x.GameEntered -= _state.OnGameEntered);
    }

    [Test]
    public void OnEnter()
    {
        _gameRepo.Reset();
        _gameRepo.Setup(x => x.SetIsMouseCaptured(false));

        _state.Enter();

        _gameRepo.VerifyAll();
    }

    [Test]
    public void OnGameEntered()
    {
        _state.Attach(_context);

        _state.OnGameEntered();

        _context.Inputs.First().Should().BeOfType<GameLogic.Input.Start>();
    }

    [Test]
    public void OnStartGame()
    {
        var result = _state.On(new GameLogic.Input.Start());
        result.State.Should().BeOfType<GameLogic.State.Playing>();
    }

    [Test]
    public void OnInitialize()
    {
        var pointsToWin = 3;
        _gameRepo.Reset();
        _gameRepo.Setup(x => x.SetPointsToWin(pointsToWin));

        var result = _state.On(new GameLogic.Input.Initialize(pointsToWin));

        result.State.Should().Be(_state);
        _gameRepo.VerifyAll();
    }
}
