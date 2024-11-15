namespace PaddleBall.Tests;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Godot;
using Moq;

public class SplashTest(Node testScene) : TestClass(testScene)
{
    private Mock<IAnimationPlayer> animationPlayer = default!;
    private Mock<IAppRepo> appRepo = default!;

    private Splash splash = default!;

    [Setup]
    public void Setup()
    {
        appRepo = new Mock<IAppRepo>();
        animationPlayer = new Mock<IAnimationPlayer>();
        splash = new Splash()
        {
            AnimationPlayer = animationPlayer.Object
        };

        splash.FakeDependency(appRepo.Object);

        splash._Notification(-1);
    }

    [Test]
    public void PlaysSplashScreen()
    {
        splash.OnReady();

        animationPlayer.VerifyAdd(
          player => player.AnimationFinished += splash.OnAnimationFinished
        );

        appRepo.Setup(repo => repo.SkipSplashScreen());
        splash.OnAnimationFinished("splash");
        appRepo.VerifyAll();

        splash.OnExitTree();
        animationPlayer.VerifyRemove(
          player => player.AnimationFinished -= splash.OnAnimationFinished
        );
    }

    [Test]
    public void SkipsSplashScreen()
    {
        splash.OnReady();

        animationPlayer.VerifyAdd(
          player => player.AnimationFinished += splash.OnAnimationFinished
        );

        // Make sure clicking skips it.
        var input = new InputEventMouseButton()
        {
            Pressed = true
        };

        appRepo.Setup(repo => repo.SkipSplashScreen());
        splash._Input(input);
        appRepo.VerifyAll();
        appRepo.Reset();

        // Make sure other inputs don't skip it.
        var otherInput = new InputEventMouseButton()
        {
            Pressed = false
        };

        var otherInput2 = new InputEventKey()
        {
            Pressed = true
        };
        splash._Input(otherInput);
        splash._Input(otherInput2);
        appRepo.VerifyAll();
    }
}
