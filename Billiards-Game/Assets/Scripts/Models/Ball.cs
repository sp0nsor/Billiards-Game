using UnityEngine;

public enum BallType { Black, White };
[RequireComponent(typeof(SphereCollider))]
public class Ball : MonoBehaviour
{
    [SerializeField] private BallType ballType;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Shot(float shotPower, float shotAngle) 
    {
        Vector3 shotForce = Quaternion.Euler(0, shotAngle, 0) * new Vector3(0, 0, shotPower / 10 * 0.9f);
        _rb.AddForce(shotForce, ForceMode.Impulse);
    }

    public BallType GetBallType()
    {
        return ballType;
    }

    public bool IsMoving()
    {
        return _rb.velocity.magnitude != 0;
    }
}
