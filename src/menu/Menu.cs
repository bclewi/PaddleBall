namespace PaddleBall;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IMenu : IControl
{
    event Menu.NewGameEventHandler NewGame;
    event Menu.QuitGameEventHandler QuitGame;
}

[Meta(typeof(IAutoNode))]
public partial class Menu : Control, IMenu
{
    public override void _Notification(int what) => this.Notify(what);

    [Node] public IButton NewGameButton { get; set; } = default!;
    [Node] public IButton QuitGameButton { get; set; } = default!;

    [Signal] public delegate void NewGameEventHandler();
    [Signal] public delegate void QuitGameEventHandler();

    public void OnReady()
    {
        NewGameButton.Pressed += OnNewGamePressed;
        QuitGameButton.Pressed += OnQuitGamePressed;
    }

    public void OnExitTree()
    {
        NewGameButton.Pressed -= OnNewGamePressed;
        QuitGameButton.Pressed -= OnQuitGamePressed;
    }

    public void OnNewGamePressed() => EmitSignal(SignalName.NewGame);
    public void OnQuitGamePressed() => GetTree().Quit();
}
