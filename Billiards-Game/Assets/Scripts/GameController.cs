using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private Game game;

    [SerializeField] private GameView view;
    [SerializeField] private BallsContainer _ballsContainer;

    private readonly WaitForSeconds _waitForBallsStopTime = new(1f);    

    private void Awake()
    {
        game = new Game();

        PocketController.OnBallPocketed += CheckPocketedBall;
        StrikeBallController.OnShotEnded += StartWaitForBallsToStop;
    }
    
    public void CheckPocketedBall(BallController ballController)
    {
        BallType ballType = ballController.GetBallType();

        game.UpdateGameState(ballType);
        view.UpdateUI(game.PocketedBallsP1, game.PocketedBallsP2, game.GameState);
    }

    private void SelectBall()
    {
        if (game.GameState == GameState.Player1Turn)
            _ballsContainer.GetStrikeBall(BallType.White).enabled = true;

        if (game.GameState == GameState.Player2Turn)
            _ballsContainer.GetStrikeBall(BallType.Black).enabled = true;
    }

    public IEnumerator WaitForBallsToStop()
    {
        yield return _waitForBallsStopTime;

        while (_ballsContainer.AreBallsMoving())
        {
            yield return _waitForBallsStopTime;
        }

        game.CheckChangeTurn();
        SelectBall();
    }

    private void StartWaitForBallsToStop()
    {
        StartCoroutine(WaitForBallsToStop());
    }

    private void OnDestroy()
    {
        PocketController.OnBallPocketed -= CheckPocketedBall;
    }
}
