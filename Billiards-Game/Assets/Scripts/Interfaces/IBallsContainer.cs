public interface IBallsContainer
{
    bool AreBallsMoving();
    Ball GetStrikeBall(BallType ballType);
    void OnBallPocketed(Ball ball);
}