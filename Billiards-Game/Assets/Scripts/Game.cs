public enum GameState { Start, Player1Turn, Player2Turn, End }
public class Game
{
    public GameState GameState { get; private set; }
    public int PocketedBallsP1 { get; private set; }
    public int PocketedBallsP2 { get; private set; }

    private bool _isBallInPocket;

    public Game()
    {
        GameState = GameState.Player1Turn;

        PocketedBallsP1 = 0;
        PocketedBallsP2 = 0;
    }

    public void UpdateGameState(BallType ballType)
    {
        _isBallInPocket = true;

        switch (ballType)
        {
            case BallType.Black:
                PocketedBallsP1++;
                GameState = GameState.Player1Turn;
                break;
            case BallType.White:
                PocketedBallsP2++;
                GameState = GameState.Player2Turn;
                break;
        }
    }

    public void CheckChangeTurn()
    {
        if (!_isBallInPocket)
            GameState = GameState == GameState.Player1Turn ? GameState.Player2Turn : GameState.Player1Turn;

        _isBallInPocket = false;
    }
}
