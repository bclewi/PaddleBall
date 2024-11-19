namespace PaddleBall.Tests;

using System.Linq;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using FluentAssertions;
using Godot;
using Moq;

public class WonTest(Node testScene) : TestClass(testScene)
{
    private Mock<IAppRepo> _appRepo = default!;

    private IFakeContext _context = default!;

    private GameLogic.State.Won _state = default!;

    [Setup]
    public void Setup()
    {
        _state = new GameLogic.State.Won();
        _context = _state.CreateFakeContext();

        _appRepo = new Mock<IAppRepo>();
        _context.Set(_appRepo.Object);
    }

    [Test]
    public void OnEnter()
    {
        _state.Enter();
        _context.Outputs.First().Should().BeOfType<GameLogic.Output.ShowWonScreen>();
    }

    [Test]
    public void OnGoToMainMenu()
    {
        _appRepo.Setup(x => x.OnExitGame(PostGameAction.GoToMainMenu));

        var result = _state.On(new GameLogic.Input.GoToMainMenu());

        _appRepo.VerifyAll();
        result.State.Should().Be(_state);
    }
}
