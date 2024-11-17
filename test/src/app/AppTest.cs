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
}
