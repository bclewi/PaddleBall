namespace PaddleBall;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IGame : INode2D, IProvide<IGameRepo>;

[Meta(typeof(IAutoNode))]
public partial class Game : Node2D, IGame
{
    public override void _Notification(int what) => this.Notify(what);

    //[Dependency] public IAppRepo AppRepo => this.DependOn<IAppRepo>();

    //[Node] public IPlayer Player { get; set; } = default!;
    //[Node] public IInGameUI InGameUi { get; set; } = default!;
    //[Node] public IWinMenu WinMenu { get; set; } = default!;
    //[Node] public IPauseMenu PauseMenu { get; set; } = default!;

    public IGameRepo GameRepo { get; set; } = default!;
    //public IGameLogic GameLogic { get; set; } = default!;
    //public GameLogic.IBinding GameBinding { get; set; } = default!;

    IGameRepo IProvide<IGameRepo>.Value() => GameRepo;

    //public void Setup() => throw new NotImplementedException();
    //public void OnResolved() => throw new NotImplementedException();
    //public override void _Input(InputEvent @event) => throw new NotImplementedException();
    //public void OnMainMenu() => throw new NotImplementedException();
    //public void OnResume() => throw new NotImplementedException();
    //public void OnStart() => throw new NotImplementedException();
    //public void OnWinMenuTransitioned() => throw new NotImplementedException();
    //public void OnPauseMenuTransitioned() => throw new NotImplementedException();
    //public void OnExitTree() => throw new NotImplementedException();
    //public void SetPauseMode(bool isPaused) => throw new NotImplementedException();
}
