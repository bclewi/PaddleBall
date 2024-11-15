namespace PaddleBall.Tests;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using FluentAssertions;
using Godot;
using Moq;

public class InGameUITest(Node testScene) : TestClass(testScene)
{
    private Mock<IAppRepo> appRepo = default!;
    private Mock<IGameRepo> gameRepo = default!;
    private Mock<IInGameUILogic> logic = default!;
    private Mock<ILabel> leftPlayerScoreLabel = default!;
    private Mock<ILabel> rightPlayerScoreLabel = default!;
    private InGameUILogic.IFakeBinding binding = default!;

    private InGameUI ui = default!;

    [Setup]
    public void Setup()
    {
        appRepo = new();
        gameRepo = new();
        logic = new();
        leftPlayerScoreLabel = new();
        rightPlayerScoreLabel = new();
        binding = InGameUILogic.CreateFakeBinding();

        logic.Setup(x => x.Bind()).Returns(binding);
        logic.Setup(x => x.Start());

        ui = new()
        {
            InGameUILogic = logic.Object
        };

        ui.FakeDependency(appRepo.Object);
        ui.FakeDependency(gameRepo.Object);

        ui._Notification(-1);
    }

    [Test]
    public void Initializes()
    {
        ui.Setup();

        ui.InGameUILogic.Should().BeOfType<InGameUILogic>();
    }

    [Test]
    public void OnExitTree()
    {
        logic.Reset();
        logic.Setup(x => x.Stop());
        ui.InGameUIBinding = binding;

        ui.OnExitTree();

        logic.VerifyAll();
    }

    [Test]
    public void ScoreChanged()
    {
        ui.OnResolved();

        leftPlayerScoreLabel.SetupSet(x => x.Text = "1");
        rightPlayerScoreLabel.SetupSet(x => x.Text = "2");

        binding.Output(new InGameUILogic.Output.ScoreChanged
        {
            PlayerOneScore = 1,
            PlayerTwoScore = 2
        });

        leftPlayerScoreLabel.VerifyAll();
        rightPlayerScoreLabel.VerifyAll();
    }
}
