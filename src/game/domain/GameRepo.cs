namespace PaddleBall;

using Chickensoft.Collections;
using Godot;
using System;

public interface IGameRepo : IDisposable
{
    /// <summary>Event invoked when the game ends.</summary>
    event Action<GameOverReason>? OnGameEnded;

    /// <summary>Event invoked when a point is scored, passing the player number.</summary>
    event Action<int>? OnPlayerScored;

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

    /// <summary>Inform the game that the player scored a point.</summary>
    /// <param name="playerNumber">Player that scored.</param>
    void Score(int playerNumber);

    /// <summary>Tells the game how many points the player has scored.</summary>
    /// <param name="playerNumber">Player number to update score.</param>
    /// <param name="points">Number of points scored.</param>
    void SetPlayerScore(int playerNumber, int points);

    /// <summary>Inform the game that the game ended.</summary>
    /// <param name="reason">Game over reason.</param>
    void EndGame(GameOverReason reason);

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
    void SetPlayerGlobalPosition(int playerNumber, Vector2 playerGlobalPosition);

    /// <summary>Tells the game how many points are needed to win the game.</summary>
    /// <param name="pointsToWin">Number of points to win the game.</param>
    void SetPointsToWin(int pointsToWin);
}

/// <summary>
///   Game repository — stores pure game logic that's not directly related to the
///   game node's overall view.
/// </summary>
public class GameRepo : IGameRepo
{
    public IAutoProp<int> PointsToWin => _pointsToWin;
    private readonly AutoProp<int> _pointsToWin;

    public IAutoProp<bool> IsMouseCaptured => _isMouseCaptured;
    private readonly AutoProp<bool> _isMouseCaptured;

    public IAutoProp<bool> IsPaused => _isPaused;
    private readonly AutoProp<bool> _isPaused;

    public IAutoProp<int> PlayerOneScore => _playerOneScore;
    private readonly AutoProp<int> _playerOneScore;

    public IAutoProp<int> PlayerTwoScore => _playerTwoScore;
    private readonly AutoProp<int> _playerTwoScore;

    public IAutoProp<Vector2> PlayerOneGlobalPosition => _playerOneGlobalPosition;
    private readonly AutoProp<Vector2> _playerOneGlobalPosition;

    public IAutoProp<Vector2> PlayerTwoGlobalPosition => _playerTwoGlobalPosition;
    private readonly AutoProp<Vector2> _playerTwoGlobalPosition;

    public event Action<GameOverReason>? OnGameEnded;
    public event Action<int>? OnPlayerScored;

    public GameRepo(int pointsToWin)
    {
        _pointsToWin = new AutoProp<int>(pointsToWin);
        _isMouseCaptured = new AutoProp<bool>(false);
        _isPaused = new AutoProp<bool>(false);
        _playerOneScore = new AutoProp<int>(0);
        _playerTwoScore = new AutoProp<int>(0);
        _playerOneGlobalPosition = new AutoProp<Vector2>(Vector2.Zero);
        _playerTwoGlobalPosition = new AutoProp<Vector2>(Vector2.Zero);
    }

    internal GameRepo
    (
        AutoProp<int> pointsToWin,
        AutoProp<bool> isMouseCaptured,
        AutoProp<bool> isPaused,
        AutoProp<int> playerOneScore,
        AutoProp<int> playerTwoScore,
        AutoProp<Vector2> playerOneGlobalPosition,
        AutoProp<Vector2> playerTwoGlobalPosition
    )
    {
        _pointsToWin = pointsToWin;

        _isMouseCaptured = isMouseCaptured;
        _isPaused = isPaused;
        _playerOneScore = playerOneScore;
        _playerTwoScore = playerTwoScore;
        _playerOneGlobalPosition = playerOneGlobalPosition;
        _playerTwoGlobalPosition = playerTwoGlobalPosition;
    }

    public void Score(int playerNumber)
    {
        var currentScore = 0;

        if (playerNumber == 1)
        {
            _playerOneScore.OnNext(_playerOneScore.Value + 1);
            OnPlayerScored?.Invoke(playerNumber);
        }
        else if (playerNumber == 2)
        {
            _playerTwoScore.OnNext(_playerTwoScore.Value + 1);
            OnPlayerScored?.Invoke(playerNumber);
        }

        if (currentScore >= _pointsToWin.Value)
        {
            EndGame(GameOverReason.Won);
        }
    }

    public void EndGame(GameOverReason reason)
    {
        _isMouseCaptured.OnNext(false);
        Pause();
        OnGameEnded?.Invoke(reason);
    }

    public void Pause()
    {
        _isMouseCaptured.OnNext(false);
        _isPaused.OnNext(true);
    }

    public void Resume()
    {
        _isMouseCaptured.OnNext(true);
        _isPaused.OnNext(false);
    }

    public void SetIsMouseCaptured(bool isMouseCaptured) => _isMouseCaptured.OnNext(isMouseCaptured);

    public void SetPlayerGlobalPosition(int playerNumber, Vector2 playerGlobalPosition)
    {
        if (playerNumber == 1)
        {
            _playerOneGlobalPosition.OnNext(playerGlobalPosition);
            return;
        }

        if (playerNumber == 2)
        {
            _playerTwoGlobalPosition.OnNext(playerGlobalPosition);
            return;
        }
    }

    public void SetPlayerScore(int playerNumber, int points)
    {
        if (playerNumber == 1)
        {
            _playerOneScore.OnNext(_playerOneScore.Value + points);
            OnPlayerScored?.Invoke(playerNumber);
            return;
        }

        if (playerNumber == 2)
        {
            _playerTwoScore.OnNext(_playerTwoScore.Value + points);
            OnPlayerScored?.Invoke(playerNumber);
            return;
        }
    }

    public void SetPointsToWin(int pointsToWin) => _pointsToWin.OnNext(pointsToWin);

    #region Dispose

    private bool disposedValue;

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
