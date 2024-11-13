namespace PaddleBall.Tests;

using System.Threading.Tasks;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using FluentAssertions;
using Godot;
using Moq;

public class MenuTest(Node testScene) : TestClass(testScene)
{
    private Mock<IButton> startGameButton = default!;
    private Mock<IButton> quitGameButton = default!;
    private Menu menu = default!;

    [Setup]
    public void Setup()
    {
        startGameButton = new Mock<IButton>();
        quitGameButton = new Mock<IButton>();

        menu = new Menu
        {
            StartGameButton = startGameButton.Object,
            QuitGameButton = quitGameButton.Object
        };

        menu._Notification(-1);
    }

    [Test]
    public void Subscribes()
    {
        menu.OnReady();
        startGameButton.VerifyAdd(x => x.Pressed += menu.OnStartGamePressed);

        menu.OnExitTree();
        startGameButton.VerifyRemove(x => x.Pressed -= menu.OnStartGamePressed);
    }

    [Test]
    public async Task SignalsStartGameButtonPressed()
    {
        var signal = menu.ToSignal(menu, Menu.SignalName.StartGame);

        menu.OnStartGamePressed();

        await signal;
        signal.IsCompleted.Should().BeTrue();
    }

    [Test]
    public async Task SignalsQuitGameButtonPressed()
    {
        var signal = menu.ToSignal(menu, Menu.SignalName.QuitGame);

        menu.OnQuitGamePressed();

        await signal;
        signal.IsCompleted.Should().BeTrue();
    }
}
