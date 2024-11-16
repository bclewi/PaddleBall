namespace PaddleBall.Tests;

using System.Threading;
using System.Threading.Tasks;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using FluentAssertions;
using Godot;
using Moq;

public class PauseMenuTest(Node testScene) : TestClass(testScene)
{
    private Mock<IButton> _mainMenuButton = default!;
    private Mock<IButton> _resumeButton = default!;
    private Mock<IAnimationPlayer> _animationPlayer = default!;

    private PauseMenu menu = default!;

    [Setup]
    public void Setup()
    {
        _mainMenuButton = new();
        _resumeButton = new();
        _animationPlayer = new();

        menu = new PauseMenu
        {
            MainMenuButton = _mainMenuButton.Object,
            ResumeButton = _resumeButton.Object,
            AnimationPlayer = _animationPlayer.Object
        };

        menu._Notification(-1);
    }

    [Test]
    public void Subscribes()
    {
        menu.OnReady();
        _mainMenuButton.VerifyAdd(x => x.Pressed += menu.OnMainMenuPressed);
        _resumeButton.VerifyAdd(x => x.Pressed += menu.OnResumePressed);
        _animationPlayer.VerifyAdd(x => x.AnimationFinished += menu.OnAnimationFinished);

        menu.OnExitTree();
        _mainMenuButton.VerifyRemove(x => x.Pressed -= menu.OnMainMenuPressed);
        _resumeButton.VerifyRemove(x => x.Pressed -= menu.OnResumePressed);
        _animationPlayer.VerifyRemove(x => x.AnimationFinished -= menu.OnAnimationFinished);
    }

    [Test]
    public async Task SignalsMainMenuButtonPressed()
    {
        var signal = menu.ToSignal(menu, PauseMenu.SignalName.MainMenu);

        menu.OnMainMenuPressed();

        await signal;
        signal.IsCompleted.Should().BeTrue();
    }

    [Test]
    public async Task SignalsResumeButtonPressed()
    {
        var signal = menu.ToSignal(menu, PauseMenu.SignalName.Resume);

        menu.OnResumePressed();

        await signal;
        signal.IsCompleted.Should().BeTrue();
    }

    [Test]
    public async Task SignalsTransitionCompleted()
    {
        var signal = menu.ToSignal(menu, PauseMenu.SignalName.TransitionCompleted);

        menu.OnAnimationFinished(PauseMenu.AnimationNames.FadeIn);

        await signal;
        signal.IsCompleted.Should().BeTrue();
    }

    [Test]
    public void FadesIn()
    {
        var customBlend = -1;
        var customSpeed = 1;
        var fromEnd = false;

        _animationPlayer.Setup(x => x.Play(PauseMenu.AnimationNames.FadeIn,
            customBlend, customSpeed, fromEnd));

        menu.FadeIn();

        _animationPlayer.VerifyAll();
    }

    [Test]
    public void FadesOut()
    {
        var customBlend = -1;
        var customSpeed = 1;
        var fromEnd = false;

        _animationPlayer.Setup(x => x.Play(PauseMenu.AnimationNames.FadeOut,
            customBlend, customSpeed, fromEnd));

        menu.FadeIn();

        _animationPlayer.VerifyAll();
    }
}
