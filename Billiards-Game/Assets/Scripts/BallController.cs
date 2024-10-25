using UnityEngine;

public enum BallType { Black, White };
[RequireComponent(typeof(SphereCollider))]
public class BallController : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private BallType ballType;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Shot(float shotPower, float shotAngle)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        Vector3 shotForce = Quaternion.Euler(0, shotAngle, 0) * new Vector3(0, 0, shotPower / 10 * 0.9f);

        rigidbody.AddForce(shotForce, ForceMode.Impulse);
    }

    private void RoundSpeed()
    {
        if (_rb.velocity.sqrMagnitude <= 0.002f)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    public BallType GetBallType()
    {
        return ballType;
    }

    public bool IsMoving()
    {
        RoundSpeed(); // подумать как сделать по другому 
        return _rb.velocity.magnitude != 0;
    }
}
