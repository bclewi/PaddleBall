namespace PaddleBall;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IInGameUI : IControl
{
    void SetPlayerScoreLabels(int playerOneScore, int playerTwoScore);
}

[Meta(typeof(IAutoNode))]
public partial class InGameUI : Control, IInGameUI
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency] public IAppRepo AppRepo => this.DependOn<IAppRepo>();
    [Dependency] public IGameRepo GameRepo => this.DependOn<IGameRepo>();

    [Node] public ILabel LeftPlayerScoreLabel { get; set; } = default!;
    [Node] public ILabel RightPlayerScoreLabel { get; set; } = default!;

    public IInGameUILogic InGameUILogic { get; set; } = default!;

    public InGameUILogic.IBinding InGameUIBinding { get; set; } = default!;

    public void Setup() => InGameUILogic = new InGameUILogic();

    public void OnResolved()
    {
        InGameUILogic.Set(this);
        InGameUILogic.Set(AppRepo);
        InGameUILogic.Set(GameRepo);

        InGameUIBinding = InGameUILogic.Bind();

        InGameUIBinding
            .Handle((in InGameUILogic.Output.ScoreChanged output)
                => SetPlayerScoreLabels(output.PlayerOneScore, output.PlayerTwoScore));

        InGameUILogic.Start();
    }

    public void SetPlayerScoreLabels(int playerOneScore, int playerTwoScore)
    {
        LeftPlayerScoreLabel.Text = playerOneScore.ToString();
        RightPlayerScoreLabel.Text = playerTwoScore.ToString();
    }

    public void OnExitTree()
    {
        InGameUILogic.Stop();
        InGameUIBinding.Dispose();
    }
}
