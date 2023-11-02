using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnabledStrikeBall : MonoBehaviour
{
    private StrikeBall strikeBall;
    private BallController ballController;
    private void OnMouseDown()
    {
        strikeBall = GetComponent<StrikeBall>();
        ballController = GetComponent<BallController>();

        if (strikeBall != null)
        {
            if (StrikeBall.CurrentActiveBall != null)
            {
                StrikeBall.CurrentActiveBall.DisableController();
            }
            if (GameController.instance.AreBallsMoving() == false)
            {
                if (GameController.instance.GetGameState() == GameState.PLAYER1TURN && ballController.getBallType() == BallType.WHITE)
                {
                    strikeBall.EnabledController();
                    StrikeBall.SetCurrentActiveBall(strikeBall);
                }
                if (GameController.instance.GetGameState() == GameState.PLAYER2TURN && ballController.getBallType() == BallType.BLACK)
                {
                    strikeBall.EnabledController();
                    StrikeBall.SetCurrentActiveBall(strikeBall);
                }
            }
        }
    }
}