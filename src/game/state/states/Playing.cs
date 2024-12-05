namespace PaddleBall;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic
{
    public partial record State
    {
        [Meta]
        public partial record Playing : State, IGet<Input.EndGame>, IGet<Input.PauseButtonPressed>
        {
            public Playing()
            {
                this.OnEnter(() =>
                {
                    Output(new Output.StartGame());
                    Get<IGameRepo>().SetIsMouseCaptured(true);
                });

                OnAttach(() => Get<IGameRepo>().OnGameEnded += OnEnded);
                OnDetach(() => Get<IGameRepo>().OnGameEnded -= OnEnded);
            }

            public void OnEnded(GameOverReason reason) => Input(new Input.EndGame(reason));

            public Transition On(in Input.EndGame input)
            {
                Get<IGameRepo>().Pause();

                return input.Reason switch
                {
                    GameOverReason.Won => To<Won>(),
                    GameOverReason.Quit => To<Quit>(),
                    _ => throw new NotImplementedException()
                };
            }

            public Transition On(in Input.PauseButtonPressed input) => To<Paused>();
        }
    }
}
