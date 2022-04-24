using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {START, PLAYER1TURN, PLAYER2TURN, END}
public class GameController : MonoBehaviour
{
    [SerializeField] private GameState _gameState = GameState.START;
    private GameState previousState = GameState.START;
    [SerializeField] private bool foul, playerPocketedBall;
    [SerializeField] private BallType player1BType = BallType.NULL, player2BType = BallType.NULL;
    [SerializeField] private PocketType player1ChosenPocket, player2ChosenPocket;
    [SerializeField] private LayerMask tableLayer;
    [SerializeField] private Vector3 tableCenter;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private List<int> P1PocketedBalls = new List<int>(), P2PocketedBalls = new List<int>();
    public static GameController instance;

    private void Awake() {
        if(instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        _uiManager.DisableMenus();
    }
    void Start()
    {
        PhysicsController.instance.SetDefaultPhysics();
        foul = false;
        playerPocketedBall = false;
        _gameState = GameState.PLAYER1TURN;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(_uiManager != null)
            {
                _uiManager.ShowGameMenu();
            }
        }
    }

    public void CheckPocketedBall(BallController ballController, PocketType pocketType)
    {
        BallType ballType = ballController.getBallType();
        BallType otherBallType = ballType == BallType.HALF ? BallType.FULL : BallType.HALF;
        if(player1BType == BallType.NULL)
        {
            if(ballType != BallType.WHITE && ballType != BallType.BLACK)
            {
                player1BType = _gameState == GameState.PLAYER1TURN ? ballType : otherBallType;
                player2BType = _gameState == GameState.PLAYER1TURN ? otherBallType : ballType;
                return;
            }
            if(ballType == BallType.WHITE)
            {
                foul = true;
                _gameState = _gameState == GameState.PLAYER1TURN ? GameState.PLAYER2TURN : GameState.PLAYER1TURN;
            }
        }
        else
        {
            switch (ballType)
            {
                case BallType.HALF:
                    if(player1BType == BallType.HALF)
                        P1PocketedBalls.Add(ballController.getBallNumber());
                    else
                        P2PocketedBalls.Add(ballController.getBallNumber());
                    break;
                case BallType.FULL:
                    if(player1BType == BallType.FULL)
                        P1PocketedBalls.Add(ballController.getBallNumber());
                    else
                        P2PocketedBalls.Add(ballController.getBallNumber());
                    break;
                case BallType.WHITE:
                    _gameState = _gameState == GameState.PLAYER1TURN ? GameState.PLAYER2TURN : GameState.PLAYER1TURN;
                    foul = true;
                    break;
                case BallType.BLACK:
                    string winner = "";
                    if(_gameState == GameState.PLAYER1TURN)
                    {
                        winner = P1PocketedBalls.Count == 7 && !foul ? "Player 1" : "Player 2";
                        P1PocketedBalls.Add(ballController.getBallNumber()); 
                    }
                    if(_gameState == GameState.PLAYER2TURN)
                    {
                        winner = P2PocketedBalls.Count == 7 && !foul ? "Player 2" : "Player 1";
                        P2PocketedBalls.Add(ballController.getBallNumber());
                    }
                    _gameState = GameState.END;
                    // Update UI
                    // UI show message Player X wins!.
                    break;
            }
        }
    }
    public void OnWhiteBallFirstHit(WhiteBallController whiteBallController, BallType otherBallType)
    {
        if(player1BType != BallType.NULL)
        {
        if(_gameState == GameState.PLAYER1TURN && player1BType != otherBallType)
        {
            _gameState = GameState.PLAYER2TURN;
            foul = true;
            return;
        }
        if(_gameState == GameState.PLAYER2TURN && player2BType != otherBallType)
        {
            _gameState = GameState.PLAYER1TURN;
            foul = true;
            return;
        }
        }

    }
    public void Foul()
    {

    }
    public LayerMask WhatIsTable()
    {
        return tableLayer;
    }
    public void EndFoul()
    {
        foul = false;
    }
    public bool IsFoul()
    {
        return foul;
    }
}
