namespace PaddleBall;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IGame : INode2D, IProvide<IGameRepo>;

[Meta(typeof(IAutoNode))]
public partial class Game : Node2D, IGame
{
    public static class InputAction
    {
        public const string UiCancel = "ui_cancel";
    }

    public override void _Notification(int what) => this.Notify(what);

    [Dependency] public IAppRepo AppRepo => this.DependOn<IAppRepo>();

    //[Node] public IPlayer Player { get; set; } = default!;
    [Node] public IInGameUI InGameUi { get; set; } = default!;
    [Node] public IWinMenu WinMenu { get; set; } = default!;
    [Node] public IPauseMenu PauseMenu { get; set; } = default!;

    public IGameRepo GameRepo { get; set; } = default!;
    public IGameLogic GameLogic { get; set; } = default!;
    public GameLogic.IBinding GameBinding { get; set; } = default!;

    IGameRepo IProvide<IGameRepo>.Value() => GameRepo;

    [Export] private int pointsToWin = 5;

    public void Setup()
    {
        GameRepo = new GameRepo(pointsToWin);
        GameLogic = new GameLogic();
        GameLogic.Set(GameRepo);
        GameLogic.Set(AppRepo);

        WinMenu.MainMenu += OnMainMenu;
        WinMenu.TransitionCompleted += OnWinMenuTransitioned;

        PauseMenu.MainMenu += OnMainMenu;
        PauseMenu.Resume += OnResume;
        PauseMenu.TransitionCompleted += OnPauseMenuTransitioned;
    }

    public void OnResolved()
    {
        GameBinding = GameLogic.Bind();

        GameBinding
            .Handle((in GameLogic.Output.StartGame _) =>
                InGameUi.Show()
            )
            .Handle((in GameLogic.Output.SetPauseMode output) =>
                CallDeferred(nameof(SetPauseMode), output.IsPaused)
            )
            .Handle((in GameLogic.Output.CaptureMouse output) =>
                Input.MouseMode = output.IsMouseCaptured
                    ? Input.MouseModeEnum.Captured
                    : Input.MouseModeEnum.Visible
            )
            .Handle((in GameLogic.Output.ShowPauseMenu _) =>
            {
                PauseMenu.Show();
                PauseMenu.FadeIn();
            })
            .Handle((in GameLogic.Output.ShowWonScreen _) =>
            {
                WinMenu.Show();
                WinMenu.FadeIn();
            })
            .Handle((in GameLogic.Output.ExitWonScreen _) =>
                WinMenu.FadeOut()
            )
            .Handle((in GameLogic.Output.ExitPauseMenu _) =>
                PauseMenu.FadeOut()
            )
            .Handle((in GameLogic.Output.HidePauseMenu _) =>
                PauseMenu.Hide()
            );

        // Trigger the first state's OnEnter callbacks so our bindings run.
        // Keeps everything in sync from the moment we start!
        GameLogic.Start();

        GameLogic.Input(new GameLogic.Input.Initialize(pointsToWin));

        this.Provide();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed(InputAction.UiCancel))
        {
            GameLogic.Input(new GameLogic.Input.PauseButtonPressed());
        }
    }

    public void OnMainMenu() => GameLogic.Input(new GameLogic.Input.GoToMainMenu());

    public void OnResume() => GameLogic.Input(new GameLogic.Input.PauseButtonPressed());

    public void OnStart() => GameLogic.Input(new GameLogic.Input.Start());

    public void OnWinMenuTransitioned() => GameLogic.Input(new GameLogic.Input.WinMenuTransitioned());

    public void OnPauseMenuTransitioned() => GameLogic.Input(new GameLogic.Input.PauseMenuTransitioned());

    public void OnExitTree()
    {
        WinMenu.MainMenu -= OnMainMenu;
        PauseMenu.MainMenu -= OnMainMenu;
        PauseMenu.Resume -= OnResume;
        PauseMenu.TransitionCompleted -= OnPauseMenuTransitioned;

        GameLogic.Stop();
        GameBinding.Dispose();
        GameRepo.Dispose();
    }

    public void SetPauseMode(bool isPaused) => GetTree().Paused = isPaused;
}
