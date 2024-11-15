namespace PaddleBall;

using Chickensoft.Collections;
using Godot;
using System;

public interface IGameRepo : IDisposable
{
    /// <summary>Event invoked when the game ends.</summary>
    event Action<GameOverReason>? Ended;

    /// <summary>Event invoked when a point is scored, passing the player number.</summary>
    event Action<int>? Scored;

    /// <summary>Mouse captured status.</summary>
    IAutoProp<bool> IsMouseCaptured { get; }

    /// <summary>Pause status.</summary>
    IAutoProp<bool> IsPaused { get; }

    /// <summary>Number of points scored by player one.</summary>
    IAutoProp<int> PlayerOneScore { get; }

    /// <summary>Number of points scored by player two.</summary>
    IAutoProp<int> PlayerTwoScore { get; }

    /// <summary>The position in global coordinates of player one.</summary>
    IAutoProp<Vector2> PlayerOneGlobalPosition { get; }

    /// <summary>The position in global coordinates of player two.</summary>
    IAutoProp<Vector2> PlayerTwoGlobalPosition { get; }

    /// <summary>Inform the game that the player is scoring a point.</summary>
    /// <param name="playerNumber">Player that is scoring.</param>
    void StartScore(int playerNumber);

    /// <summary>Inform the game that the player scored a point.</summary>
    /// <param name="playerNumber">Player that scored.</param>
    void OnFinishScore(int playerNumber);

    /// <summary>Tells the game how many points are required to win a game.</summary>
    /// <param name="scoreToWin">Number of points to win.</param>
    void SetScoreToWin(int scoreToWin);

    /// <summary>Tells the game how many points the player has scored.</summary>
    /// <param name="playerNumber">Player number to update score.</param>
    /// <param name="points">Number of points scored.</param>
    void SetPlayerScore(int playerNumber, int points);

    /// <summary>Inform the game that the game ended.</summary>
    /// <param name="reason">Game over reason.</param>
    void OnGameEnded(GameOverReason reason);

    /// <summary>Pauses the game and releases the mouse.</summary>
    void Pause();

    /// <summary>Resumes the game and recaptures the mouse.</summary>
    void Resume();

    /// <summary>Changes whether the mouse is captured or not.</summary>
    /// <param name="isMouseCaptured">
    ///   Whether or not the mouse is captured.
    /// </param>
    void SetIsMouseCaptured(bool isMouseCaptured);

    /// <summary>Sets the player's global position.</summary>
    /// <param name="playerNumber">Player number to update.</param>
    /// <param name="playerGlobalPosition">
    ///   Player's global position in world
    ///   coordinates.
    /// </param>
    void SetPlayerGlobalPosition(int playerNumber, Vector3 playerGlobalPosition);
}

/// <summary>
///   Game repository — stores pure game logic that's not directly related to the
///   game node's overall view.
/// </summary>
public class GameRepo : IGameRepo
{
    private bool disposedValue;

    public IAutoProp<bool> IsMouseCaptured => throw new NotImplementedException();
    public IAutoProp<bool> IsPaused => throw new NotImplementedException();
    public IAutoProp<int> PlayerOneScore => throw new NotImplementedException();
    public IAutoProp<int> PlayerTwoScore => throw new NotImplementedException();
    public IAutoProp<Vector2> PlayerOneGlobalPosition => throw new NotImplementedException();
    public IAutoProp<Vector2> PlayerTwoGlobalPosition => throw new NotImplementedException();

    public event Action<GameOverReason>? Ended;
    public event Action<int>? Scored;

    public void OnFinishScore(int playerNumber) => throw new NotImplementedException();
    public void OnGameEnded(GameOverReason reason) => throw new NotImplementedException();
    public void Pause() => throw new NotImplementedException();
    public void Resume() => throw new NotImplementedException();
    public void SetIsMouseCaptured(bool isMouseCaptured) => throw new NotImplementedException();
    public void SetPlayerGlobalPosition(int playerNumber, Vector3 playerGlobalPosition) => throw new NotImplementedException();
    public void SetPlayerScore(int playerNumber, int points) => throw new NotImplementedException();
    public void SetScoreToWin(int scoreToWin) => throw new NotImplementedException();
    public void StartScore(int playerNumber) => throw new NotImplementedException();

    #region Dispose

    protected void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: Dispose managed objects
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion Dispose
}
