namespace PaddleBall;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IMenu : IControl
{
    event Menu.StartGameEventHandler StartGame;
    event Menu.QuitGameEventHandler QuitGame;
}

[Meta(typeof(IAutoNode))]
public partial class Menu : Control, IMenu
{
    public override void _Notification(int what) => this.Notify(what);

    [Node] public IButton StartGameButton { get; set; } = default!;
    [Node] public IButton QuitGameButton { get; set; } = default!;

    [Signal] public delegate void StartGameEventHandler();
    [Signal] public delegate void QuitGameEventHandler();

    public void OnReady()
    {
        StartGameButton.Pressed += OnStartGamePressed;
        QuitGameButton.Pressed += OnQuitGamePressed;
    }

    public void OnExitTree()
    {
        StartGameButton.Pressed -= OnStartGamePressed;
        QuitGameButton.Pressed -= OnQuitGamePressed;
    }

    public void OnStartGamePressed() => EmitSignal(SignalName.StartGame);
    public void OnQuitGamePressed() => EmitSignal(SignalName.QuitGame);
}
