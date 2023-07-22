using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnabledWhiteBallController: MonoBehaviour
{
    private WhiteBallController whiteBallController;

    private void Start()
    {
        whiteBallController = GetComponent<WhiteBallController>();
    }

    private void OnMouseDown()
    {
        whiteBallController.EnabledController();
    }
}
