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

        SelectBall();
    }

    public void CheckPocketedBall(Ball ballController)
    {
        BallType ballType = ballController.GetBallType();
        game.UpdateGameState(ballType);
    }

    private void SelectBall()
    {
        if (!TryGetBall(out var ball))
            return;

        _gameView.UpdateCurrentBall(ball);
    }

    private bool TryGetBall(out Ball ball)
    {
        ball = game.GameState switch
        {
            GameState.Player1Turn => _ballsContainer.GetStrikeBall(BallType.White),
            GameState.Player2Turn => _ballsContainer.GetStrikeBall(BallType.Black),
            _ => null
        };

        return ball != null;
    }

    public IEnumerator WaitForBallsToStop()
    {
        yield return _waitForBallsStopTime;

        while (_ballsContainer.AreBallsMoving())
        {
            yield return _waitForBallsStopTime;
        }

        game.CheckChangeTurn();
        _matchView.UpdateUI(game.PocketedBallsP1, game.PocketedBallsP2, game.GameState);
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
