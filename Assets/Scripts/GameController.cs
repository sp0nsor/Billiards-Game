using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

public enum GameState { Start, Player1Turn, Player2Turn, End }
public class GameController : MonoBehaviour
{
    [FormerlySerializedAs("Player1Balls")] [SerializeField] private List<StrikeBall> player1Balls = new List<StrikeBall>(8);
    [FormerlySerializedAs("Player2Balls")] [SerializeField] private List<StrikeBall> player2Balls = new List<StrikeBall>(8);
    [FormerlySerializedAs("Balls")] [SerializeField] private List<BallController> balls = new List<BallController>(16);
    [FormerlySerializedAs("P1PocketedBalls")] [SerializeField] private List<int> p1PocketedBalls = new List<int>(8);
    [FormerlySerializedAs("P2PocketedBalls")] [SerializeField] private List<int> p2PocketedBalls = new List<int>(8);
    [FormerlySerializedAs("_gameState")] [SerializeField] private GameState gameState = GameState.Start;
    [FormerlySerializedAs("_uiManager")] [SerializeField] private UIManager uiManager;
    [SerializeField] private BallType player1BType, player2BType;
    [SerializeField] private LayerMask tableLayer;
    private readonly WaitForSeconds _waitForBallsStopTime = new(1f);
    private StrikeBall _activePlayer1Ball, _activePlayer2Ball;
    private bool _coroutineIsRunning = false;
    private bool _ballInPocket = false;
    public static GameController Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

    }

    private void Start()
    {
        PhysicsController.Instance.SetDefaultPhysics();
        uiManager.DisableMenus();
        player1BType = BallType.White;
        player2BType = BallType.Black;
        gameState = GameState.Player1Turn;
    }
    
    public void CheckPocketedBall(BallController ballController)
    {
        _ballInPocket = true;
        BallType ballType = ballController.GetBallType();
        switch (ballType)
        {
            case BallType.Black:
                if (player1BType == BallType.Black)
                {
                    p1PocketedBalls.Add(ballController.GetBallNumber());
                }
                else
                {
                    p2PocketedBalls.Add(ballController.GetBallNumber());
                }
                UpdateGameAfterBallRemoval(ballController, GameState.Player1Turn);
                break;
            case BallType.White:
                if (player1BType == BallType.White)
                {
                    p1PocketedBalls.Add(ballController.GetBallNumber());
                }
                else
                {
                    p2PocketedBalls.Add(ballController.GetBallNumber());
                }
                UpdateGameAfterBallRemoval(ballController, GameState.Player2Turn);
                break;
        }
        if (p1PocketedBalls.Count == 8) { ExecuteEndGameSequence(ballController, "Player 2 wins"); }
        if (p2PocketedBalls.Count == 8) { ExecuteEndGameSequence(ballController, "Player 1 wins"); }
    }

    private void UpdateGameAfterBallRemoval(BallController ballController, GameState newState)
    {
        RemoveFromBalls(ballController);
        uiManager.UpdateUI(p1PocketedBalls, p2PocketedBalls);
        if (!_coroutineIsRunning) { StartCoroutine(WaitForBallsToStopAndChangeTurn(newState)); }
    }

    private void ExecuteEndGameSequence(BallController ballController, String winMessage)
    {
        uiManager.OnGameEnd(winMessage);
        RemoveFromBalls(ballController);
        gameState = GameState.End;
    }

    private IEnumerator WaitForBallsToStopAndChangeTurn(GameState newState)
    {
        _coroutineIsRunning = true;
        yield return _waitForBallsStopTime;

        while (AreBallsMoving())
        {
            yield return _waitForBallsStopTime;
        }

        _coroutineIsRunning = false;
        gameState = newState;
        uiManager.UpdateUI(p1PocketedBalls, p2PocketedBalls);
        SelectBall();
    }
    
    private StrikeBall GetBall(List<StrikeBall> balls)
    {
        balls.RemoveAll(ball => ball == null);
        return balls[0];
    }
    
    private void SelectBall()
    {
        if (StrikeBall.CurrentActiveBall != null)
        {
            StrikeBall.CurrentActiveBall.DisableController();
        }
        if (gameState == GameState.Player1Turn)
        {
            _activePlayer1Ball = GetBall(player1Balls);
            _activePlayer1Ball.EnabledController();
            _activePlayer1Ball.EnabledStick();
            StrikeBall.SetCurrentActiveBall(_activePlayer1Ball);
        }
        if (gameState == GameState.Player2Turn)
        {
            _activePlayer2Ball = GetBall(player2Balls);
            _activePlayer2Ball.EnabledController();
            _activePlayer2Ball.EnabledStick();
            StrikeBall.SetCurrentActiveBall(_activePlayer2Ball);
        }
    }
    
    public void CheckChangeTurn()
    {
        if (!_ballInPocket)
        {
            gameState = gameState == GameState.Player1Turn ? GameState.Player2Turn : GameState.Player1Turn;
            uiManager.UpdateUI(p1PocketedBalls, p2PocketedBalls);
            SelectBall();
            return;
        }
        _ballInPocket = false;
    }
    
    public LayerMask WhatIsTable()
    {
        return tableLayer;
    }
    
    public bool AreBallsMoving()
    {
        foreach (BallController ballController in balls)
        {
            if (ballController.IsMoving())
            {
                return true;
            }
        }
        return false;
    }
    
    public IEnumerator WaitForBallsToStop()
    {
        yield return _waitForBallsStopTime;

        while (AreBallsMoving())
        {
            yield return _waitForBallsStopTime;
        }
        CheckChangeTurn();
    }
    
    public void RemoveFromBalls(BallController ballController)
    {
        BallType ballType = ballController.GetBallType();

        List<StrikeBall> strikeBalls = ballType == player1BType ? player1Balls : player2Balls;
        strikeBalls.RemoveAll(ball => ball.GetBallController() == ballController);

        balls.Remove(ballController);
    }
    
    public GameState GetGameState()
    {
        return gameState;
    }
}
