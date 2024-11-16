namespace PaddleBall;

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
    public static class AnimationName
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
        MainMenuButton.Pressed += OnMainMenuPressed;
        ResumeButton.Pressed += OnResumePressed;
        AnimationPlayer.AnimationFinished += OnAnimationFinished;
    }

    public void OnExitTree()
    {
        MainMenuButton.Pressed -= OnMainMenuPressed;
        ResumeButton.Pressed -= OnResumePressed;
        AnimationPlayer.AnimationFinished -= OnAnimationFinished;
    }

    public void OnMainMenuPressed() => EmitSignal(SignalName.MainMenu);
    public void OnResumePressed() => EmitSignal(SignalName.Resume);
    public void OnAnimationFinished(StringName _) => EmitSignal(SignalName.TransitionCompleted);

    public void FadeIn() => AnimationPlayer.Play(AnimationName.FadeIn);
    public void FadeOut() => AnimationPlayer.Play(AnimationName.FadeOut);
}
