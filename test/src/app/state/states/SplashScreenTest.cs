namespace PaddleBall;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using FluentAssertions;
using Godot;
using Moq;

public class SplashScreenTest(Node testScene) : TestClass(testScene)
{
    private Mock<IAppRepo> _appRepo = default!;
    private IFakeContext _context = default!;

    private AppLogic.State.SplashScreen state = default!;

    [Setup]
    public void Setup()
    {
        state = new();
        _appRepo = new();
        _context = state.CreateFakeContext();
        _context.Set(_appRepo.Object);
    }

    [Test]
    public void OnEnter()
    {
        state.Enter();

        _context.Outputs.Should().ContainInConsecutiveOrder([new AppLogic.Output.ShowSplashScreen()]);
    }

    [Test]
    public void Subscribes()
    {
        state.Attach(_context);

        _appRepo.VerifyAdd(x => x.SplashScreenSkipped += state.OnSplashScreenSkipped);

        // Likewise, we pass the parent class to the exit method as the next state
        // to prevent us from "leaving" the current state, which prevents the parent
        // exit callbacks from running.
        state.Detach();

        _appRepo.VerifyRemove(x => x.SplashScreenSkipped -= state.OnSplashScreenSkipped);
    }

    [Test]
    public void RespondsToFadeOutFinished()
    {
        var next = state.On(new AppLogic.Input.FadeOutFinished());

        next.State.Should().BeOfType<AppLogic.State.MainMenu>();
    }

    [Test]
    public void SkipsSplashScreen()
    {
        state.OnSplashScreenSkipped();

        _context.Outputs.Should().ContainInConsecutiveOrder([new AppLogic.Output.HideSplashScreen()]);
    }
}
