namespace PaddleBall;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface ISplash : IControl;

[Meta(typeof(IAutoNode))]
public partial class Splash : Control, ISplash
{
    public override void _Notification(int what) => this.Notify(what);

    [Node] public IAnimationPlayer AnimationPlayer { get; set; } = default!;

    [Dependency] public IAppRepo AppRepo => this.DependOn<IAppRepo>();

    public void OnReady() => AnimationPlayer.AnimationFinished += OnAnimationFinished;
    public void OnExitTree() => AnimationPlayer.AnimationFinished -= OnAnimationFinished;

    public void OnAnimationFinished(StringName name) => AppRepo.SkipSplashScreen();

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            AppRepo.SkipSplashScreen();
        }
    }
}
