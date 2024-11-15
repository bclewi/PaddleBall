namespace PaddleBall;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IWinMenu : IControl
{
    event WinMenu.MainMenuEventHandler MainMenu;
    event WinMenu.TransitionCompletedEventHandler TransitionCompleted;

    void FadeIn();
    void FadeOut();
}

[Meta(typeof(IAutoNode))]
public partial class WinMenu : Control, IWinMenu
{
    public override void _Notification(int what) => this.Notify(what);

    [Node] public IButton MainMenuButton { get; set; } = default!;
    [Node] public IAnimationPlayer AnimationPlayer { get; set; } = default!;

    [Signal] public delegate void MainMenuEventHandler();
    [Signal] public delegate void TransitionCompletedEventHandler();

    public void OnReady() => MainMenuButton.Pressed += OnMainMenuPressed;
    public void OnExitTree() => MainMenuButton.Pressed -= OnMainMenuPressed;

    public void OnMainMenuPressed() => EmitSignal(SignalName.MainMenu);
    public void OnAnimationFinished(StringName name) => EmitSignal(SignalName.TransitionCompleted);

    public void FadeIn() => AnimationPlayer.Play("fade_in");
    public void FadeOut() => AnimationPlayer.Play("fade_out");
}
