using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class EnabledWhiteBallController: MonoBehaviour
{
    private WhiteBallController whiteBallController;

    private void OnMouseDown()
    {
        whiteBallController = GetComponent<WhiteBallController>();

        if (whiteBallController != null)
        {
            if(WhiteBallController.CurrentActiveBall != null)
            {
                WhiteBallController.CurrentActiveBall.DisableController();
            }
            whiteBallController.EnabledController();
            WhiteBallController.SetCurrentActiveBall(whiteBallController);
        }
    }
}
