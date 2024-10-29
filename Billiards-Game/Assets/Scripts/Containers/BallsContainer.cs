using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class BallsContainer : MonoBehaviour, IBallsContainer
{
    private List<Ball> balls = new List<Ball>();

    private void Awake()
    {
        PocketController.OnBallPocketed += OnBallPocketed;
        balls = new List<Ball>(16);

        InitBalls();
    }

    private void InitBalls()
    {
        foreach (Transform ball in transform)
            balls.Add(ball.GetComponent<Ball>());
    }

    public void OnBallPocketed(Ball ball)
    {
        balls.Remove(ball);

        Destroy(ball.gameObject, 0.3f);
    }

    public bool AreBallsMoving()
    {
        foreach (Ball ballController in balls)
            if (ballController.IsMoving())
                return true;

        return false;
    }

    public Ball GetStrikeBall(BallType ballType)
    {
        return balls.First(b => b.GetBallType() == ballType);
    }

    private void OnDestroy()
    {
        PocketController.OnBallPocketed -= OnBallPocketed;
    }
}
