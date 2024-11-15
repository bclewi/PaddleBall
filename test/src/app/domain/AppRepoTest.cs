namespace PaddleBall.Tests;

using System;
using Chickensoft.GoDotTest;
using FluentAssertions;
using Godot;

public class AppRepoTest(Node testScene) : TestClass(testScene)
{
    private AppRepo repo = default!;

    [Setup]
    public void Setup() => repo = new();

    [Cleanup]
    public void Cleanup() => repo.Dispose();

    [Test]
    public void Initializes()
    {
        var repo = new AppRepo();
        repo.Should().BeAssignableTo<IAppRepo>();
    }

    [Test]
    public void SkipSplashScreen()
    {
        var called = false;

        void splashScreenSkipped() => called = true;

        // invoke event without handlers to cover null check
        repo.SkipSplashScreen();

        repo.SplashScreenSkipped += splashScreenSkipped;

        repo.SkipSplashScreen();

        called.Should().BeTrue();
    }

    [Test]
    public void OnMainMenuEnteredInvokesEvent()
    {
        var called = 0;

        void onMainMenuEntered() => called++;

        repo.OnMainMenuEntered();
        repo.MainMenuEntered += onMainMenuEntered;
        repo.OnMainMenuEntered();

        called.Should().Be(1);
    }

    [Test]
    public void OnEnterGameInvokesEvent()
    {
        var called = 0;

        void onEnterGame() => called++;

        repo.OnEnterGame();
        repo.GameEntered += onEnterGame;
        repo.OnEnterGame();

        called.Should().Be(1);
    }

    [Test]
    public void OnExitGameInvokesEventWithPostGameAction()
    {
        var called = 0;
        var expectedActionType = PostGameAction.GoToMainMenu;

        void onExitGame(PostGameAction actionType)
        {
            called++;
            actionType.Should().Be(expectedActionType);
        }

        repo.OnExitGame(expectedActionType);
        repo.GameExited += onExitGame;
        repo.OnExitGame(expectedActionType);

        called.Should().Be(1);
    }

    [Test]
    public void Disposes()
    {
        Action action = repo.Dispose;

        action.Should().NotThrow();
        // Redundant dispose shouldn't do anything.
        action.Should().NotThrow();
    }
}
