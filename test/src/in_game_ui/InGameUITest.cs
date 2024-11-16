namespace PaddleBall.Tests;

using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using FluentAssertions;
using Godot;
using Moq;

public class InGameUITest(Node testScene) : TestClass(testScene)
{
    private Mock<IAppRepo> _appRepo = default!;
    private Mock<IGameRepo> _gameRepo = default!;
    private Mock<ILabel> _leftPlayerScoreLabel = default!;
    private Mock<ILabel> _rightPlayerScoreLabel = default!;

    private Mock<IInGameUILogic> logic = default!;
    private InGameUILogic.IFakeBinding binding = default!;

    private InGameUI ui = default!;

    [Setup]
    public void Setup()
    {
        _appRepo = new();
        _gameRepo = new();
        _leftPlayerScoreLabel = new();
        _rightPlayerScoreLabel = new();

        logic = new();
        binding = InGameUILogic.CreateFakeBinding();

        logic.Setup(x => x.Bind()).Returns(binding);
        logic.Setup(x => x.Start());

        ui = new()
        {
            LeftPlayerScoreLabel = _leftPlayerScoreLabel.Object,
            RightPlayerScoreLabel = _rightPlayerScoreLabel.Object,
            InGameUILogic = logic.Object
        };

        ui.FakeDependency(_appRepo.Object);
        ui.FakeDependency(_gameRepo.Object);

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

        _gameRepo.Setup(x => x.PlayerTwoScore).Returns(new AutoProp<int>(1));

        _leftPlayerScoreLabel.SetupSet(x => x.Text = "1");
        _rightPlayerScoreLabel.SetupSet(x => x.Text = "2");

        binding.Output(new InGameUILogic.Output.ScoreChanged
        {
            PlayerOneScore = 1,
            PlayerTwoScore = 2
        });

        _leftPlayerScoreLabel.VerifyAll();
        _rightPlayerScoreLabel.VerifyAll();
    }
}
