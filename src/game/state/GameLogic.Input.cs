namespace PaddleBall;

public partial class GameLogic
{
    public static class Input
    {
        public readonly record struct Initialize(int PointsToWin);

        public readonly record struct EndGame(GameOverReason Reason);

        public readonly record struct PauseButtonPressed;

        public readonly record struct PauseMenuTransitioned;

        public readonly record struct WinMenuTransitioned;

        public readonly record struct GoToMainMenu;

        public readonly record struct Start;
    }
}
