using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum GameState {START, PLAYER1TURN, PLAYER2TURN, FOUL, END}
    public GameState _gameState = GameState.START;
    public BallType player1BType = BallType.NULL;
    public BallType player2BType = BallType.NULL;
    public static GameController instance;
    public LayerMask tableLayer;
    [SerializeField] private UIManager _uiManager;

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

    public void OnBallPocket(int ballNumber)
    {
        //when ball goes into pocket
    }
    public void OnWhiteBallFirstHit(WhiteBallController whiteBallController, BallType otherBallType)
    {
        if(_gameState == GameState.PLAYER1TURN && player1BType != otherBallType)
        {
            _gameState = GameState.PLAYER2TURN;
        }

    }
    public void Foul()
    {

    }
}
