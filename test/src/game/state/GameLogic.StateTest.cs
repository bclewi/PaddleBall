namespace PaddleBall.Tests;

using Chickensoft.Collections;
using Chickensoft.GoDotTest;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using FluentAssertions;
using Godot;
using Moq;

public partial class GameLogicStateTest(Node testScene) : TestClass(testScene)
{
    [Meta]
    public partial record TestGameState : GameLogic.State;

    private Mock<IGameRepo> _gameRepo = default!;
    private Mock<IAutoProp<bool>> _isMouseCaptured = default!;
    private Mock<IAutoProp<bool>> _isPaused = default!;

    private IFakeContext _context = default!;

    private GameLogic.State _state = default!;

    [Setup]
    public void Setup()
    {
        _gameRepo = new();

        _state = new TestGameState();
        _context = _state.CreateFakeContext();
        _context.Set(_gameRepo.Object);

        _isMouseCaptured = new();
        _isPaused = new();

        _gameRepo.Setup(x => x.IsMouseCaptured).Returns(_isMouseCaptured.Object);
        _gameRepo.Setup(x => x.IsPaused).Returns(_isPaused.Object);
    }

    [Test]
    public void Subscribes()
    {
        _state.Attach(_context);

        _gameRepo.VerifyAdd(x => x.IsMouseCaptured.Sync += _state.OnIsMouseCaptured);
        _gameRepo.VerifyAdd(x => x.IsPaused.Sync += _state.OnIsPaused);

        _state.Detach();

        _gameRepo.VerifyRemove(x => x.IsMouseCaptured.Sync -= _state.OnIsMouseCaptured);
        _gameRepo.VerifyRemove(x => x.IsPaused.Sync -= _state.OnIsPaused);
    }

    [Test]
    public void OnIsMouseCaptured()
    {
        _state.OnIsMouseCaptured(true);

        _context.Outputs.Should().BeEquivalentTo(new object[] { new GameLogic.Output.CaptureMouse(true) });
    }

    [Test]
    public void OnIsPaused()
    {
        _state.OnIsPaused(true);

        _context.Outputs.Should().BeEquivalentTo(new object[] { new GameLogic.Output.SetPauseMode(true) });
    }
}
