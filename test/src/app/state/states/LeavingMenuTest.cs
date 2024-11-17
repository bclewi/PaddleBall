namespace PaddleBall;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using FluentAssertions;
using Godot;
using Moq;

public class LeavingMenuTest(Node testScene) : TestClass(testScene)
{
    private Mock<IAppRepo> _appRepo = default!;
    private IFakeContext _context = default!;

    private AppLogic.State.LeavingMenu _state = default!;

    [Setup]
    public void Setup()
    {
        _state = new();
        _appRepo = new();
        _context = _state.CreateFakeContext();
        _context.Set(_appRepo.Object);
    }

    [Test]
    public void Enters()
    {
        _state.Enter();

        _context.Outputs.Should().ContainInConsecutiveOrder([new AppLogic.Output.FadeToBlack()]);
    }

    [Test]
    public void StartsGameOnFadeOutFinished()
    {
        var next = _state.On(new AppLogic.Input.FadeOutFinished());

        next.State.Should().BeOfType<AppLogic.State.InGame>();
    }
}
