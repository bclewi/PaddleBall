namespace PaddleBall;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using FluentAssertions;
using Godot;
using Moq;

public class AppTest(Node testScene) : TestClass(testScene)
{
    private Mock<IAppRepo> _appRepo = default!;
    private Mock<IAppLogic> _logic = default!;
    private Mock<IInstantiator> _instantiator = default!;
    private Mock<IGame> _game = default!;
    private Mock<IMenu> _menu = default!;
    private Mock<ISubViewport> _gamePreview = default!;
    private Mock<IColorRect> _blankScreen = default!;
    private Mock<IAnimationPlayer> _animationPlayer = default!;
    private Mock<ISplash> _splash = default!;
    private AppLogic.IFakeBinding _binding = default!;

    private App _app = default!;

    [Setup]
    public void Setup()
    {
        _appRepo = new();
        _logic = new();
        _binding = AppLogic.CreateFakeBinding();

        _instantiator = new();

        _game = new();
        _menu = new();
        _gamePreview = new();
        _blankScreen = new();
        _animationPlayer = new();
        _splash = new();

        _app = new()
        {
            AppRepo = _appRepo.Object,
            AppLogic = _logic.Object,
            Game = _game.Object,
            Instantiator = _instantiator.Object,
            Menu = _menu.Object,
            GamePreview = _gamePreview.Object,
            BlankScreen = _blankScreen.Object,
            AnimationPlayer = _animationPlayer.Object,
            Splash = _splash.Object
        };

        (_app as IAutoInit).IsTesting = true;

        _logic.Setup(logic => logic.Bind()).Returns(_binding);
        _logic.Setup(logic => logic.Start());
    }

    [Test]
    public void Initializes()
    {
        // Naturally, the app controls al ot of systems (mostly menus), so there's
        // quite a bit of setup to verify.

        _app.AppBinding = _binding;

        _app.Initialize();

        _app.Instantiator.Should().BeOfType<Instantiator>();
        _app.AppRepo.Should().BeOfType<AppRepo>();
        _app.AppLogic.Should().BeOfType<AppLogic>();

        _menu.VerifyAdd(x => x.NewGame += _app.OnNewGame);
        _animationPlayer.VerifyAdd(x => x.AnimationFinished += _app.OnAnimationFinished);

        // Make sure the app provides dependency values to its descendants.
        (_app as IProvider).ProviderState.IsInitialized.Should().BeTrue();
        (_app as IProvide<IAppRepo>).Value().Should().Be(_app.AppRepo);

        // Make sure it cleans everything up.

        _app.OnExitTree();

        _menu.VerifyRemove(x => x.NewGame -= _app.OnNewGame);
        _animationPlayer.VerifyRemove(x => x.AnimationFinished -= _app.OnAnimationFinished);

        _app._Notification(-1);
    }

    [Test]
    public void ShowsSplashScreen()
    {
        SetupHideMenus();
        _blankScreen.Setup(x => x.Hide());
        _splash.Setup(x => x.Show());

        _app.OnReady();

        _binding.Output(new AppLogic.Output.ShowSplashScreen());

        _blankScreen.VerifyAll();
        _splash.VerifyAll();
    }

    [Test]
    public void HidesSplashScreen()
    {
        _blankScreen.Setup(x => x.Show());
        SetupFadeOut();

        _app.OnReady();

        _binding.Output(new AppLogic.Output.HideSplashScreen());

        _blankScreen.VerifyAll();
    }

    [Test]
    public void RemovesExistingGame()
    {
        _game.Setup(x => x.QueueFree());

        _app.Game = _game.Object;
        _gamePreview.Setup(x => x.RemoveChildEx(_game.Object));

        _app.OnReady();

        _binding.Output(new AppLogic.Output.RemoveExistingGame());

        _gamePreview.VerifyAll();

        _app.Game.Should().BeNull();

        _game.VerifyAll();
    }

    [Test]
    public void LoadsGame()
    {
        var game = new Game();

        _instantiator.Setup(x => x.LoadAndInstantiate<Game>(It.IsAny<string>())).Returns(game);
        _instantiator.Setup(x => x.SceneTree).Returns(TestScene.GetTree());
        _gamePreview.Setup(x => x.AddChildEx(game, false, Node.InternalMode.Disabled));

        _app.OnReady();

        _binding.Output(new AppLogic.Output.SetupGameScene());

        _instantiator.VerifyAll();
        _gamePreview.VerifyAll();
    }

    [Test]
    public void ShowsMainMenu()
    {
        SetupHideMenus();
        _menu.Setup(x => x.Show());
        _game.Setup(x => x.Show());
        SetupFadeIn();

        _app.OnReady();

        _binding.Output(new AppLogic.Output.ShowMainMenu());

        _menu.VerifyAll();
        _game.VerifyAll();
        _blankScreen.VerifyAll();
        _animationPlayer.VerifyAll();
    }

    [Test]
    public void FadesOut()
    {
        SetupFadeOut();

        _app.OnReady();

        _binding.Output(new AppLogic.Output.FadeToBlack());

        VerifyFade();
    }

    [Test]
    public void ShowsGame()
    {
        SetupHideMenus();

        SetupFadeIn();

        _app.OnReady();

        _binding.Output(new AppLogic.Output.ShowGame());

        VerifyFade();
    }

    [Test]
    public void HidesGame()
    {
        SetupFadeOut();

        _app.OnReady();

        _binding.Output(new AppLogic.Output.HideGame());

        VerifyFade();
    }

    [Test]
    public void OnNewGameWorks()
    {
        _logic.Reset();
        _logic.Setup(x => x.Input(It.IsAny<AppLogic.Input.NewGame>()));

        _app.OnNewGame();

        _logic.VerifyAll();
    }

    [Test]
    public void OnAnimationFinishedRespondsToFadeInFinished()
    {
        _logic.Reset();
        _logic.Setup(x => x.Input(It.IsAny<AppLogic.Input.FadeInFinished>()));
        _blankScreen.Setup(screen => screen.Hide());

        _app.OnAnimationFinished(App.AnimationName.FadeIn);

        _logic.VerifyAll();
        _blankScreen.VerifyAll();
    }

    [Test]
    public void OnAnimationFinishedRespondsToFadeOutFinished()
    {
        _logic.Reset();
        _logic.Setup(x => x.Input(It.IsAny<AppLogic.Input.FadeOutFinished>()));

        _app.OnAnimationFinished(App.AnimationName.FadeOut);

        _logic.VerifyAll();
    }

    private void SetupHideMenus()
    {
        _menu.Setup(x => x.Hide());
        _splash.Setup(x => x.Hide());
    }

    private void SetupFadeIn()
    {
        _blankScreen.Setup(x => x.Show());
        _animationPlayer.Setup(x => x.Play(App.AnimationName.FadeIn, -1, 1, false));
    }

    private void SetupFadeOut()
    {
        _blankScreen.Setup(x => x.Show());
        _animationPlayer.Setup(x => x.Play(App.AnimationName.FadeOut, -1, 1, false));
    }

    private void VerifyFade()
    {
        _blankScreen.VerifyAll();
        _animationPlayer.VerifyAll();
    }
}
