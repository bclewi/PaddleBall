namespace PaddleBall;

public partial class InGameUILogic
{
    public static class Output
    {
        public readonly record struct ScoreChanged
        {
            public readonly required int PlayerOneScore { get; init; }
            public readonly required int PlayerTwoScore { get; init; }
        }
    }
}
