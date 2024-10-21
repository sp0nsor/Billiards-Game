using System;
using UnityEngine;

public class StrikeBallController : MonoBehaviour
{
    public static event Action OnShotEnded;
    public static event Action<bool, Vector3> OnVisibilityStatusChange;

    [SerializeField] private StrikeBallView StrikeBallView;

    private float _currentYaw;

    private void Shot(float shotPower, float shotAngle)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        Vector3 shotForce = Quaternion.Euler(0, shotAngle, 0) * new Vector3(0, 0, shotPower / 10 * 0.9f);

        rigidbody.AddForce(shotForce, ForceMode.Impulse);

        OnShotEnded?.Invoke();
        enabled = false;
    }

    private void CalculateDegree(Vector3 to)
    {
        Vector3 vectorBetween = to - transform.position;
        float degree = Mathf.Atan2(vectorBetween.x, vectorBetween.z) * Mathf.Rad2Deg;
        _currentYaw = (degree + 360f) % 360f;

        StrikeBallView.SetStickPosition(transform.position);
        StrikeBallView.RotateStickAroundBall(transform.position, _currentYaw);
    }

    private void OnEnable()
    {
        StrikeBallView.OnShotEnded += Shot;
        StrikeBallView.OnTouchDetected += CalculateDegree;
        OnVisibilityStatusChange?.Invoke(true, transform.position);
    }

    private void OnDisable()
    {
        StrikeBallView.OnShotEnded -= Shot;
        StrikeBallView.OnTouchDetected -= CalculateDegree;
        OnVisibilityStatusChange?.Invoke(false, transform.position);
    }
}
