using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { START, PLAYER1TURN, PLAYER2TURN, END }
public class GameController : MonoBehaviour
{
    [SerializeField] private GameState _gameState = GameState.START;
    [SerializeField] private GameState _previousState = GameState.START;
    [SerializeField] private bool foul;
    [SerializeField] private BallType player1BType, player2BType;
    [SerializeField] private LayerMask tableLayer;
    [SerializeField] private Vector3 waitingPoint;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private List<int> P1PocketedBalls = new List<int>(), P2PocketedBalls = new List<int>();
    [SerializeField] private List<BallController> Balls = new List<BallController>();
    [SerializeField] private List<StrikeBall> Player1Balls = new List<StrikeBall>(), Player2Balls = new List<StrikeBall>();
    private StrikeBall activePlayer1Ball, activePlayer2Ball;
    private System.Random random = new System.Random();
    private bool didPocketOwnBall = false;
    public static GameController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        foul = false;
        _gameState = GameState.PLAYER1TURN;

    }
    void Start()
    {
        _uiManager.DisableMenus();
        PhysicsController.instance.SetDefaultPhysics();
        player1BType = BallType.WHITE;
        player2BType = BallType.BLACK;
    }
    public void CheckPocketedBall(BallController ballController)
    {
        BallType ballType = ballController.getBallType();
        if (P1PocketedBalls.Count == 7)
        {
            _uiManager.OnGameEnd("Player 2 wins");
            RemoveFromBalls(ballController);
            _gameState = GameState.END;
        }
        if (P2PocketedBalls.Count == 7)
        {
            _uiManager.OnGameEnd("Player 1 wins");
            RemoveFromBalls(ballController);
            _gameState = GameState.END;
        }
        switch (ballType)
        {
            case BallType.BLACK:
                if (player1BType == BallType.BLACK)
                {
                    P1PocketedBalls.Add(ballController.getBallNumber());
                    StartCoroutine(WaitForBallsToStopAndChangeTurn(GameState.PLAYER2TURN));
                    didPocketOwnBall = true;
                }
                else
                {
                    P2PocketedBalls.Add(ballController.getBallNumber());
                    StartCoroutine(WaitForBallsToStopAndChangeTurn(GameState.PLAYER2TURN));
                    didPocketOwnBall = false;
                }
                RemoveFromBalls(ballController);
                _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
                break;
            case BallType.WHITE:
                if (player1BType == BallType.WHITE)
                {
                    P1PocketedBalls.Add(ballController.getBallNumber());
                    StartCoroutine(WaitForBallsToStopAndChangeTurn(GameState.PLAYER1TURN));
                    didPocketOwnBall = false;
                }
                else
                {
                    P2PocketedBalls.Add(ballController.getBallNumber());
                    StartCoroutine(WaitForBallsToStopAndChangeTurn(GameState.PLAYER1TURN));
                    SelectBall();
                    didPocketOwnBall = true;
                }
                RemoveFromBalls(ballController);
                _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
                break;
        }
        SelectBall();
    }
    private IEnumerator WaitForBallsToStopAndChangeTurn(GameState newState)
    {
        //yield return new WaitUntil(() => !AreBallsMoving());
        while(AreBallsMoving())
        {
            yield return null;
        }

        _gameState = newState;
    }
    private void ChangeGameState(GameState newState)
    {
        if (newState == _gameState)
        {
            SelectBall();
            return;
        }
        if (_previousState == GameState.PLAYER1TURN && newState != GameState.PLAYER2TURN ||
            _previousState == GameState.PLAYER2TURN && newState != GameState.PLAYER1TURN) 
        {
            return;
        }
        _previousState = _gameState;
        _gameState = newState;
    }
    private StrikeBall GetRandomBall(List<StrikeBall> balls)
    {
        int randomIndex = random.Next(0, balls.Count - 1);
        return balls[randomIndex];
    }
    private void SelectBall()
    {   
        if(!AreBallsMoving())
        {
            if(StrikeBall.CurrentActiveBall != null)
            {
                StrikeBall.CurrentActiveBall.DisableController();
            }
            if(_gameState == GameState.PLAYER1TURN)
            {
                activePlayer1Ball = GetRandomBall(Player1Balls);
                activePlayer1Ball.EnabledController();
                activePlayer2Ball.DisableController();
                StrikeBall.SetCurrentActiveBall(activePlayer1Ball);
            }
            if(_gameState == GameState.PLAYER2TURN)
            {
                activePlayer2Ball = GetRandomBall(Player2Balls);
                activePlayer2Ball.EnabledController();
                activePlayer1Ball.DisableController();
                StrikeBall.SetCurrentActiveBall(activePlayer2Ball);
            }
        }
    }

    public Vector3 GetWaitingPoint()
    {
        return waitingPoint;
    }
    public void CheckChangeTurn()
    {
        if (!didPocketOwnBall)
        {
            _gameState = _gameState == GameState.PLAYER1TURN ? GameState.PLAYER2TURN : GameState.PLAYER1TURN;
            _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
            ChangeGameState(_gameState);
            return;
        }
        didPocketOwnBall = false;
    }
    public LayerMask WhatIsTable()
    {
        return tableLayer;
    }
    public bool AreBallsMoving()
    {
        foreach (BallController ballController in Balls)
        {
            if (ballController.isMoving())
            {
                return true;
            }
        }
        return false;
    }
    public void RemoveFromBalls(BallController ballController)
    {
        Player1Balls.RemoveAll(ball => ball.GetBallController() == ballController);
        Player2Balls.RemoveAll(ball => ball.GetBallController() == ballController);
        Balls.Remove(ballController);
    }
    public GameState GetGameState()
    {
        return _gameState;
    }
}