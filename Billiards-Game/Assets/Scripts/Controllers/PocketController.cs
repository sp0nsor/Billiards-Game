using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PocketController : MonoBehaviour
{
    public static event Action<Ball> OnBallPocketed;

    private void OnTriggerEnter(Collider other)
    {
        Ball ballController = other.GetComponent<Ball>();
        if(ballController)
            OnBallPocketed?.Invoke(ballController);
    }
}
