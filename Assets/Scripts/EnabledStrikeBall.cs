using UnityEngine;

public class EnabledStrikeBall : MonoBehaviour
{
    private StrikeBall _strikeBall;
    private BallController _ballController;
    private void OnMouseDown()
    {
        if (GameController.Instance.AreBallsMoving() == false)
        {
            StrikeBall.CurrentActiveBall.DisableController();
            _strikeBall = GetComponent<StrikeBall>();
            _ballController = GetComponent<BallController>();

            if (GameController.Instance.GetGameState() == GameState.Player1Turn && _ballController.GetBallType() == BallType.White)
            {
                _strikeBall.EnabledController();
                StrikeBall.SetCurrentActiveBall(_strikeBall);
            }
            if (GameController.Instance.GetGameState() == GameState.Player2Turn && _ballController.GetBallType() == BallType.Black)
            {
                _strikeBall.EnabledController();
                StrikeBall.SetCurrentActiveBall(_strikeBall);
            }
        }
    }
}
