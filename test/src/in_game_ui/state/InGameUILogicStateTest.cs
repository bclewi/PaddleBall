namespace PaddleBall.Tests;

using Chickensoft.Collections;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using FluentAssertions;
using Godot;
using Moq;

public class InGameUILogicStateTest(Node testScene) : TestClass(testScene)
{
    private Mock<IInGameUI> inGameUi = default!;
    private Mock<IAppRepo> appRepo = default!;
    private Mock<IGameRepo> gameRepo = default!;
    private IFakeContext context = default!;

    private InGameUILogic.State state = default!;

    [Setup]
    public void Setup()
    {
        inGameUi = new Mock<IInGameUI>();
        appRepo = new Mock<IAppRepo>();
        gameRepo = new Mock<IGameRepo>();

        state = new InGameUILogic.State();

        context = state.CreateFakeContext();

        context.Set(inGameUi.Object);
        context.Set(appRepo.Object);
        context.Set(gameRepo.Object);
    }

    [Test]
    public void Subscribes()
    {
        var playerOneScore = new Mock<IAutoProp<int>>();
        var playerTwoScore = new Mock<IAutoProp<int>>();

        gameRepo.Setup(x => x.PlayerOneScore).Returns(playerOneScore.Object);
        gameRepo.Setup(x => x.PlayerTwoScore).Returns(playerTwoScore.Object);

        state.Attach(context);

        gameRepo.VerifyAdd(x => x.PlayerOneScore.Sync += state.OnPlayerOneScoreChanged);
        gameRepo.VerifyAdd(x => x.PlayerTwoScore.Sync += state.OnPlayerTwoScoreChanged);

        state.Detach();

        gameRepo.VerifyRemove(x => x.PlayerOneScore.Sync -= state.OnPlayerOneScoreChanged);
        gameRepo.VerifyRemove(x => x.PlayerTwoScore.Sync -= state.OnPlayerTwoScoreChanged);
    }

    [Test]
    public void OnPlayerOneScoreChangedOutputs()
    {
        gameRepo.Setup(x => x.PlayerOneScore).Returns(new AutoProp<int>(0));
        gameRepo.Setup(x => x.PlayerTwoScore).Returns(new AutoProp<int>(0));

        state.OnPlayerOneScoreChanged(1);

        context.Outputs.Should().BeEquivalentTo(new object[]
        {
            new InGameUILogic.Output.ScoreChanged
            {
                PlayerOneScore = 1,
                PlayerTwoScore = 0
            }
        });
    }

    [Test]
    public void OnPlayerTwoScoreChangedOutputs()
    {
        gameRepo.Setup(x => x.PlayerOneScore).Returns(new AutoProp<int>(0));
        gameRepo.Setup(x => x.PlayerTwoScore).Returns(new AutoProp<int>(0));

        state.OnPlayerTwoScoreChanged(1);

        context.Outputs.Should().BeEquivalentTo(new object[]
        {
            new InGameUILogic.Output.ScoreChanged
            {
                PlayerOneScore = 0,
                PlayerTwoScore = 1
            }
        });
    }
}
