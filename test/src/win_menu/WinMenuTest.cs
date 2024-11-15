namespace PaddleBall.Tests;

using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using FluentAssertions;
using Godot;
using Moq;
using System.Threading.Tasks;

public class WinMenuTest(Node testScene) : TestClass(testScene)
{
    private Mock<IButton> mainMenuButton = default!;
    private Mock<IAnimationPlayer> animationPlayer = default!;
    private WinMenu menu = default!;

    [Setup]
    public void Setup()
    {
        mainMenuButton = new Mock<IButton>();
        animationPlayer = new Mock<IAnimationPlayer>();
        menu = new WinMenu
        {
            MainMenuButton = mainMenuButton.Object,
            AnimationPlayer = animationPlayer.Object
        };

        menu._Notification(-1);
    }

    [Test]
    public void Subscribes()
    {
        menu.OnReady();
        mainMenuButton.VerifyAdd(x => x.Pressed += menu.OnMainMenuPressed);

        menu.OnExitTree();
        mainMenuButton
          .VerifyRemove(x => x.Pressed -= menu.OnMainMenuPressed);
    }

    [Test]
    public async Task SignalsMainMenuButtonPressed()
    {
        var signal = menu.ToSignal(menu, WinMenu.SignalName.MainMenu);

        menu.OnMainMenuPressed();

        await signal;

        signal.IsCompleted.Should().BeTrue();
    }

    [Test]
    public void FadeIn()
    {
        menu.FadeIn();
        animationPlayer.Verify(player => player.Play("fade_in", -1, 1, false));
    }

    [Test]
    public void FadeOut()
    {
        menu.FadeOut();
        animationPlayer.Verify(player => player.Play("fade_out", -1, 1, false));
    }

    [Test]
    public void OnAnimationFinished()
    {
        var called = false;
        menu.TransitionCompleted += () => called = true;

        menu.OnAnimationFinished("fade_in");

        called.Should().BeTrue();
    }
}
