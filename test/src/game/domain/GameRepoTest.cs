namespace PaddleBall.Tests;

using System;
using Chickensoft.Collections;
using Chickensoft.GoDotTest;
using FluentAssertions;
using Godot;

public class GameRepoTest(Node testScene) : TestClass(testScene)
{
    private AutoProp<int> _pointsToWin = default!;
    private AutoProp<bool> _isMouseCaptured = default!;
    private AutoProp<bool> _isPaused = default!;
    private AutoProp<int> _playerOneScore = default!;
    private AutoProp<int> _playerTwoScore = default!;
    private AutoProp<Vector2> _playerOneGlobalPosition = default!;
    private AutoProp<Vector2> _playerTwoGlobalPosition = default!;


    private GameRepo _repo = default!;

    [Setup]
    public void Setup()
    {
        _pointsToWin = new(3);
        _isMouseCaptured = new(false);
        _isPaused = new(false);
        _playerOneScore = new(0);
        _playerTwoScore = new(0);
        _playerOneGlobalPosition = new(Vector2.Zero);
        _playerTwoGlobalPosition = new(Vector2.Zero);

        _repo = new
        (
            _pointsToWin,
            _isMouseCaptured,
            _isPaused,
            _playerOneScore,
            _playerTwoScore,
            _playerOneGlobalPosition,
            _playerTwoGlobalPosition
        );
    }

    [Cleanup]
    public void Cleanup() => _repo.Dispose();

    [Test]
    public void Initializes()
    {
        var repo = new GameRepo(pointsToWin: 3);
        repo.Should().BeAssignableTo<IGameRepo>();
    }

    [Test]
    public void SetPlayerGlobalPosition()
    {
        _repo.SetPlayerGlobalPosition(1, Vector2.One);
        _repo.SetPlayerGlobalPosition(2, Vector2.One);

        _repo.PlayerOneGlobalPosition.Value.Should().Be(Vector2.One);
        _repo.PlayerTwoGlobalPosition.Value.Should().Be(Vector2.One);
    }

    [Test]
    public void SetIsMouseCaptured()
    {
        _repo.SetIsMouseCaptured(true);

        _repo.IsMouseCaptured.Value.Should().BeTrue();
    }

    [Test]
    public void ScoreTriggersWin()
    {
        GameOverReason gameOverReason = default!;
        var scoringPlayerNumber = 0;

        void HandleGameEnded(GameOverReason reason) => gameOverReason = reason;
        void HandlePlayerScored(int playerNumber) => scoringPlayerNumber = playerNumber;

        _repo.OnGameEnded += HandleGameEnded;
        _repo.OnPlayerScored += HandlePlayerScored;

        var playerNumber = 1;
        _repo.Score(playerNumber);
        _repo.PlayerOneScore.Value.Should().Be(1);
        scoringPlayerNumber.Should().Be(playerNumber);

        _repo.Score(playerNumber);
        _repo.PlayerOneScore.Value.Should().Be(2);
        scoringPlayerNumber.Should().Be(playerNumber);

        _repo.Score(playerNumber);
        _repo.PlayerOneScore.Value.Should().Be(3);
        scoringPlayerNumber.Should().Be(playerNumber);
        gameOverReason.Should().Be(GameOverReason.Won);

        _repo.OnGameEnded -= HandleGameEnded;
        _repo.OnPlayerScored -= HandlePlayerScored;
    }

    [Test]
    public void ScoreInvokesOnScored()
    {
        var called = false;
        void HandleScored(int _) => called = true;
        _repo.OnPlayerScored += HandleScored;

        _repo.Score(playerNumber: 1);
        called.Should().BeTrue();

        called = false;

        _repo.Score(playerNumber: 2);
        called.Should().BeTrue();

        _repo.OnPlayerScored -= HandleScored;
    }

    [Test]
    public void EndGamePausesAndInvokesOnGameEnded()
    {
        var called = false;
        _repo.OnGameEnded += _ => called = true;

        _repo.EndGame(GameOverReason.Won);

        called.Should().BeTrue();
        _repo.IsPaused.Value.Should().BeTrue();
        _repo.IsMouseCaptured.Value.Should().BeFalse();
    }

    [Test]
    public void EndGameDoesNotInvokeEventIfNoListeners()
    {
        Action action = () => _repo.EndGame(GameOverReason.Won);

        action.Should().NotThrow();
    }

    [Test]
    public void Pause()
    {
        _repo.Pause();
        _isMouseCaptured.Value.Should().BeFalse();
        _isPaused.Value.Should().BeTrue();
    }

    [Test]
    public void Resume()
    {
        _repo.Resume();
        _isMouseCaptured.Value.Should().BeTrue();
        _isPaused.Value.Should().BeFalse();
    }

    [Test]
    public void SetPlayerScore()
    {
        var scoringPlayer = 0;

        _repo.OnPlayerScored += (int playerNumber) => scoringPlayer = playerNumber;

        var playerNumber = 1;
        _repo.SetPlayerScore(playerNumber, points: 1);
        _repo.PlayerOneScore.Value.Should().Be(1);
        scoringPlayer.Should().Be(playerNumber);

        playerNumber = 2;
        _repo.SetPlayerScore(playerNumber, points: 2);
        _repo.PlayerTwoScore.Value.Should().Be(2);
        scoringPlayer.Should().Be(playerNumber);
    }

    [Test]
    public void SetPointsToWin()
    {
        _repo.SetPointsToWin(5);
        _repo.PointsToWin.Value.Should().Be(5);
    }
}
