using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallsContainer : MonoBehaviour, IBallsContainer
{
    private List<BallController> balls = new List<BallController>();

    private void Awake()
    {
        PocketController.OnBallPocketed += OnBallPocketed;
        balls = new List<BallController>(16);

        InitBalls();
    }

    private void InitBalls()
    {
        foreach (Transform ball in transform)
            balls.Add(ball.GetComponent<BallController>());
    }

    public void OnBallPocketed(BallController ball)
    {
        balls.Remove(ball);

        Destroy(ball.gameObject, 1f);
    }

    public bool AreBallsMoving()
    {
        foreach (BallController ballController in balls)
            if (ballController.IsMoving())
                return true;

        return false;
    }

    public BallController GetStrikeBall(BallType ballType)
    {
        return balls.First(b => b.GetBallType() == ballType);
    }

    private void OnDestroy()
    {
        PocketController.OnBallPocketed -= OnBallPocketed;
    }
}
