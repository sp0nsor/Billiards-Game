using Zenject;
using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    private Game game;

    private IGameView _gameView;
    private IMatchView _matchView;
    private IBallsContainer _ballsContainer;

    private readonly WaitForSeconds _waitForBallsStopTime = new(1f);

    [Inject]
    public void Construct(IGameView gameView, IMatchView matchView, IBallsContainer ballsContainer)
    {
        _gameView = gameView;
        _matchView = matchView;
        _ballsContainer = ballsContainer;
    }

    private void Awake()
    {
        game = new Game();

        PocketController.OnBallPocketed += CheckPocketedBall;
    }

    private void Start()
    {
        SelectBall();
    }

    public void CheckPocketedBall(BallController ballController)
    {
        BallType ballType = ballController.GetBallType();

        game.UpdateGameState(ballType);
        _matchView.UpdateUI(game.PocketedBallsP1, game.PocketedBallsP2, game.GameState);
    }

    private void SelectBall()
    {
        BallController ball;
        if (game.GameState == GameState.Player1Turn)
            ball = _ballsContainer.GetStrikeBall(BallType.White);
        else
            ball = _ballsContainer.GetStrikeBall(BallType.Black);

        _gameView.UpdateCurrentBall(ball);
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

    public void StartWaitForBallsToStop()
    {
        StartCoroutine(WaitForBallsToStop());
    }

    private void OnDestroy()
    {
        PocketController.OnBallPocketed -= CheckPocketedBall;
    }
}
