namespace PaddleBall;

using Chickensoft.Introspection;

public partial class AppLogic
{
    public partial record State
    {
        [Meta]
        public partial record LeavingGame : State, IGet<Input.FadeOutFinished>
        {
            public PostGameAction PostGameAction { get; set; } = PostGameAction.RestartGame;

            public Transition On(in Input.FadeOutFinished input)
            {
                if (PostGameAction is not PostGameAction.RestartGame)
                {
                    return To<MainMenu>();
                }

                Output(new Output.SetupGameScene());
                return To<InGame>();
            }
        }
    }
}
