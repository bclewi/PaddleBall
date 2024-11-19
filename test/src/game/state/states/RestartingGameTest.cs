namespace PaddleBall.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;

public class RestartingGameTest(Node testScene) : TestClass(testScene)
{
    private Mock<IAppRepo> _appRepo = default!;

    private IFakeContext _context = default!;

    private GameLogic.State.RestartingGame _state = default!;

    [Setup]
    public void Setup()
    {
        _state = new GameLogic.State.RestartingGame();
        _context = _state.CreateFakeContext();

        _appRepo = new Mock<IAppRepo>();
        _context.Set(_appRepo.Object);
    }

    [Test]
    public void OnEnter()
    {
        _appRepo.Setup(x => x.OnExitGame(PostGameAction.RestartGame));

        _state.Enter();

        _appRepo.VerifyAll();
    }
}
