using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PocketController : MonoBehaviour
{
    public static event Action<BallController> OnBallPocketed;

    private void OnTriggerEnter(Collider other)
    {
        BallController ballController = other.GetComponent<BallController>();
        if(ballController)
            OnBallPocketed?.Invoke(ballController);
    }
}
