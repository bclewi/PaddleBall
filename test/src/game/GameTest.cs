namespace PaddleBall.Tests;

using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.GoDotTest;
using Chickensoft.GodotTestDriver;
using Chickensoft.GodotTestDriver.Util;
using FluentAssertions;
using Godot;
using Moq;

public class GameTest(Node testScene) : TestClass(testScene)
{
    private Fixture _fixture = default!;

    private Mock<IAppRepo> _appRepo = default!;
    private Mock<IGameRepo> _gameRepo = default!;
    private Mock<IGameLogic> _logic = default!;
    //private Mock<IPlayer> _player = default!;
    private Mock<IInGameUI> _ui = default!;
    private Mock<IWinMenu> _winMenu = default!;
    private Mock<IPauseMenu> _pauseMenu = default!;

    private GameLogic.IFakeBinding _binding = default!;

    private Game _game = default!;

    [Setup]
    public async Task Setup()
    {
        _fixture = new(TestScene.GetTree());

        _appRepo = new();
        _gameRepo = new();
        _logic = new();
        _binding = GameLogic.CreateFakeBinding();
        //_player = new();
        _ui = new();
        _winMenu = new();
        _pauseMenu = new();

        _logic.Setup(x => x.Bind()).Returns(_binding);

        _game = new()
        {
            GameRepo = _gameRepo.Object,
            GameLogic = _logic.Object,
            GameBinding = _binding,
            InGameUi = _ui.Object,
            WinMenu = _winMenu.Object,
            PauseMenu = _pauseMenu.Object
        };

        (_game as IAutoInit).IsTesting = true;

        _game.FakeDependency(_appRepo.Object);

        _game.FakeNodeTree(new()
        {
            //["%Player"] = _player.Object,
            ["%InGameUi"] = _ui.Object,
            ["%WinMenu"] = _winMenu.Object,
            ["%PauseMenu"] = _pauseMenu.Object
        });

        await _fixture.AddToRoot(_game);
    }

    [Cleanup]
    public async Task Cleanup() => await _fixture.Cleanup();

    [Test]
    public void Initializes()
    {
        ((IProvide<IGameRepo>)_game).Value().Should().Be(_gameRepo.Object);

        _game.Setup();

        _game.GameRepo.Should().BeOfType<GameRepo>();
        _game.GameBinding.Should().Be(_binding);

        _game.OnResolved();
        // Make sure the game provided its dependencies.
        (_game as IProvider).ProviderState.IsInitialized.Should().BeTrue();
    }

    [Test]
    public void StartsGame()
    {
        _logic.Setup(x => x.Input(It.IsAny<GameLogic.Input.Initialize>()));
        _game.OnResolved();

        _binding.Output(new GameLogic.Output.StartGame());

        _logic.VerifyAll();
    }

    [Test]
    public async Task SetsPauseMode()
    {
        _game.OnResolved();
        var tree = TestScene.GetTree();
        tree.Paused.Should().BeFalse();

        _binding.Output(new GameLogic.Output.SetPauseMode(IsPaused: true));

        await tree.NextFrame();

        tree.Paused.Should().BeTrue();
        tree.Paused = false;
    }

    [Test]
    public void CapturesMouse()
    {
        _game.OnResolved();

        _binding.Output(new GameLogic.Output.CaptureMouse(true));
        Input.MouseMode.Should().Be(Input.MouseModeEnum.Captured);

        _binding.Output(new GameLogic.Output.CaptureMouse(false));
        Input.MouseMode.Should().Be(Input.MouseModeEnum.Visible);
    }

    [Test]
    public void ShowsPauseMenu()
    {
        _pauseMenu.Setup(x => x.Show());
        _pauseMenu.Setup(x => x.FadeIn());

        _game.OnResolved();

        _binding.Output(new GameLogic.Output.ShowPauseMenu());

        _pauseMenu.VerifyAll();
    }

    [Test]
    public void ShowsWonScreen()
    {
        _winMenu.Setup(x => x.Show());
        _winMenu.Setup(x => x.FadeIn());

        _game.OnResolved();

        _binding.Output(new GameLogic.Output.ShowWonScreen());

        _winMenu.VerifyAll();
    }

    [Test]
    public void ExitsWonScreen()
    {
        _game.OnResolved();

        _binding.Output(new GameLogic.Output.ExitWonScreen());

        _winMenu.Verify(x => x.FadeOut());
    }

    [Test]
    public void ExitsPauseMenu()
    {
        _game.OnResolved();

        _binding.Output(new GameLogic.Output.ExitPauseMenu());

        _pauseMenu.Verify(x => x.FadeOut());
    }

    [Test]
    public void HidesPauseMenu()
    {
        _game.OnResolved();

        _binding.Output(new GameLogic.Output.HidePauseMenu());

        _pauseMenu.Verify(x => x.Hide());
    }

    [Test]
    public void InputsPauseButtonPressed()
    {
        _logic.Setup(x => x.Input(It.IsAny<GameLogic.Input.PauseButtonPressed>()));
        Input.ActionPress(Game.InputAction.UiCancel);

        _game._Input(default!);

        _logic.VerifyAll();
    }

    [Test]
    public void OnMainMenu()
    {
        _game.OnMainMenu();

        _logic.Verify(x => x.Input(It.IsAny<GameLogic.Input.GoToMainMenu>()));
    }

    [Test]
    public void OnResume()
    {
        _game.OnResume();

        _logic.Verify(x => x.Input(It.IsAny<GameLogic.Input.PauseButtonPressed>()));
    }

    [Test]
    public void OnStart()
    {
        _game.OnStart();

        _logic.Verify(x => x.Input(It.IsAny<GameLogic.Input.Start>()));
    }

    [Test]
    public void OnWinMenuTransitioned()
    {
        _game.OnWinMenuTransitioned();

        _logic.Verify(x => x.Input(It.IsAny<GameLogic.Input.WinMenuTransitioned>()));
    }

    [Test]
    public void OnPauseMenuTransitioned()
    {
        _game.OnPauseMenuTransitioned();

        _logic.Verify(x => x.Input(It.IsAny<GameLogic.Input.PauseMenuTransitioned>()));
    }
}
