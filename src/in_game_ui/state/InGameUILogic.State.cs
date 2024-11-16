namespace PaddleBall;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class InGameUILogic
{
    [Meta]
    public partial record State : StateLogic<State>
    {
        public State()
        {
            OnAttach(() =>
            {
                var gameRepo = Get<IGameRepo>();
                gameRepo.PlayerOneScore.Sync += OnPlayerOneScoreChanged;
                gameRepo.PlayerTwoScore.Sync += OnPlayerTwoScoreChanged;
            });

            OnDetach(() =>
            {
                var gameRepo = Get<IGameRepo>();
                gameRepo.PlayerOneScore.Sync -= OnPlayerOneScoreChanged;
                gameRepo.PlayerTwoScore.Sync -= OnPlayerTwoScoreChanged;
            });
        }

        public void OnPlayerOneScoreChanged(int newScore) => Output(new Output.ScoreChanged
        {
            PlayerOneScore = newScore,
            PlayerTwoScore = Get<IGameRepo>().PlayerTwoScore.Value
        });

        public void OnPlayerTwoScoreChanged(int newScore) => Output(new Output.ScoreChanged
        {
            PlayerOneScore = Get<IGameRepo>().PlayerOneScore.Value,
            PlayerTwoScore = newScore
        });
    }
}
