public interface IBallsContainer
{
    bool AreBallsMoving();
    BallController GetStrikeBall(BallType ballType);
    void OnBallPocketed(BallController ball);
}