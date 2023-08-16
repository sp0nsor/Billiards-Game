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

        if (strikeBall != null && StrikeBall.FirstMove == false && StrikeBall.AreBallMoving == false)
        {
            if (StrikeBall.CurrentActiveBall != null)
            {
                StrikeBall.CurrentActiveBall.DisableController();
            }

            if (GameController.instance.GetGameState() == GameState.PLAYER1TURN && ballController.getBallType() == BallType.FULL)
            {
                strikeBall.EnabledController();
                StrikeBall.SetCurrentActiveBall(strikeBall);
            }
            if (GameController.instance.GetGameState() == GameState.PLAYER2TURN && ballController.getBallType() == BallType.HALF)
            {
                strikeBall.EnabledController();
                StrikeBall.SetCurrentActiveBall(strikeBall);
            }
        }
    }
}