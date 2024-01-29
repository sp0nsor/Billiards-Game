using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PocketController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        BallController ballController = other.GetComponent<BallController>();
        if (ballController)
        {
            ballController.GotPocketed();
        }
    }
}
