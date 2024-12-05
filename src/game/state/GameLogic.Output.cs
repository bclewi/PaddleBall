namespace PaddleBall;

public partial class GameLogic
{
    public static class Output
    {
        public readonly record struct StartGame;

        public readonly record struct SetPauseMode(bool IsPaused);

        public readonly record struct CaptureMouse(bool IsMouseCaptured);

        public readonly record struct ShowPauseMenu;

        public readonly record struct HidePauseMenu;

        public readonly record struct ExitPauseMenu;

        public readonly record struct ShowWonScreen;

        public readonly record struct ExitWonScreen;
    }
}