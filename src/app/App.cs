namespace PaddleBall;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IApp : ICanvasLayer, IProvide<IAppRepo>;

[Meta(typeof(IAutoNode))]
public partial class App : CanvasLayer, IApp
{
    public override void _Notification(int what) => this.Notify(what);

    public const string GameScenePath = "res://src/game/Game.tscn";

    public static class AnimationName
    {
        public const string FadeIn = "fade_in";
        public const string FadeOut = "fade_out";
    }

    public IGame Game { get; set; } = default!;
    public IInstantiator Instantiator { get; set; } = default!;

    public IAppRepo AppRepo { get; set; } = default!;
    public IAppLogic AppLogic { get; set; } = default!;

    public AppLogic.IBinding AppBinding { get; set; } = default!;

    IAppRepo IProvide<IAppRepo>.Value() => AppRepo;

    [Node] public IMenu Menu { get; set; } = default!;
    [Node] public ISubViewport GamePreview { get; set; } = default!;
    [Node] public IColorRect BlankScreen { get; set; } = default!;
    [Node] public IAnimationPlayer AnimationPlayer { get; set; } = default!;
    [Node] public ISplash Splash { get; set; } = default!;

    public void Initialize()
    {
        Instantiator = new Instantiator(GetTree());
        AppRepo = new AppRepo();
        AppLogic = new AppLogic();
        AppLogic.Set(AppRepo);

        // Listen for various menu signals. Each of these just translate to an input
        // for the overall app's state machine.
        Menu.NewGame += OnNewGame;
        AnimationPlayer.AnimationFinished += OnAnimationFinished;

        this.Provide();
    }

    public void OnReady()
    {
        AppBinding = AppLogic.Bind();

        AppBinding
          .Handle((in AppLogic.Output.ShowSplashScreen _) =>
          {
              HideMenus();
              BlankScreen.Hide();
              Splash.Show();
          })
          .Handle((in AppLogic.Output.HideSplashScreen _) =>
          {
              BlankScreen.Show();
              FadeToBlack();
          })
          .Handle((in AppLogic.Output.RemoveExistingGame _) =>
          {
              GamePreview.RemoveChildEx(Game);
              Game.QueueFree();
              Game = default!;
          })
          .Handle((in AppLogic.Output.SetupGameScene _) =>
          {
              Game = Instantiator.LoadAndInstantiate<Game>(GameScenePath);
              GamePreview.AddChildEx(Game);

              Instantiator.SceneTree.Paused = false;
          })
          .Handle((in AppLogic.Output.ShowMainMenu _) =>
          {
              HideMenus();
              Menu.Show();
              Game.Show();

              FadeInFromBlack();
          })
          .Handle((in AppLogic.Output.FadeToBlack _) => FadeToBlack())
          .Handle((in AppLogic.Output.ShowGame _) =>
          {
              HideMenus();
              FadeInFromBlack();
          })
          .Handle((in AppLogic.Output.HideGame _) => FadeToBlack());

        // Enter the first state to kick off the binding side effects.
        AppLogic.Start();
    }

    public void OnNewGame() => AppLogic.Input(new AppLogic.Input.NewGame());

    public void OnAnimationFinished(StringName animation)
    {
        if (animation == AnimationName.FadeIn)
        {
            AppLogic.Input(new AppLogic.Input.FadeInFinished());
            BlankScreen.Hide();
            return;
        }

        AppLogic.Input(new AppLogic.Input.FadeOutFinished());
    }

    public void FadeInFromBlack()
    {
        BlankScreen.Show();
        AnimationPlayer.Play(AnimationName.FadeIn);
    }

    public void FadeToBlack()
    {
        BlankScreen.Show();
        AnimationPlayer.Play(AnimationName.FadeOut);
    }

    public void HideMenus()
    {
        Splash.Hide();
        Menu.Hide();
    }

    public void OnExitTree()
    {
        AppLogic.Stop();
        AppBinding.Dispose();
        AppRepo.Dispose();

        Menu.NewGame -= OnNewGame;
        AnimationPlayer.AnimationFinished -= OnAnimationFinished;
    }
}
