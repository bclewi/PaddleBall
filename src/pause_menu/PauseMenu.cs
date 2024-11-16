namespace PaddleBall;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IPauseMenu : IControl
{
    event PauseMenu.MainMenuEventHandler MainMenu;
    event PauseMenu.ResumeEventHandler Resume;
    event PauseMenu.TransitionCompletedEventHandler TransitionCompleted;

    void FadeIn();
    void FadeOut();
}

[Meta(typeof(IAutoNode))]
public partial class PauseMenu : Control, IPauseMenu
{
    public static class AnimationNames
    {
        public const string FadeIn = "fade_in";
        public const string FadeOut = "fade_out";
    }

    public override void _Notification(int what) => this.Notify(what);

    [Node] public IButton MainMenuButton { get; set; } = default!;
    [Node] public IButton ResumeButton { get; set; } = default!;
    [Node] public IAnimationPlayer AnimationPlayer { get; set; } = default!;

    [Signal] public delegate void MainMenuEventHandler();
    [Signal] public delegate void ResumeEventHandler();
    [Signal] public delegate void TransitionCompletedEventHandler();

    public void OnReady()
    {
        AnimationPlayer.AnimationFinished += OnAnimationFinished;
    }

    public void OnExitTree()
    {
        AnimationPlayer.AnimationFinished -= OnAnimationFinished;
    }

    public void OnMainMenuPressed() => throw new NotImplementedException();
    public void OnResumePressed() => throw new NotImplementedException();
    public void OnAnimationFinished(StringName name) => throw new NotImplementedException();

    public void FadeIn() => throw new NotImplementedException();
    public void FadeOut() => throw new NotImplementedException();
}
