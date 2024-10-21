using UnityEngine;

public enum BallType { Black, White };
[RequireComponent(typeof(SphereCollider))]
public class BallController : MonoBehaviour
{
    private Rigidbody _rb;
    private AudioSource _audioSourse;
    [SerializeField] private BallType ballType;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSourse = gameObject.GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            if (_rb.velocity.sqrMagnitude > 0.004f)
            {
                PlayCollisionSound(_rb.velocity.magnitude / 2);
            }
        }
        if (other.gameObject.CompareTag("Band"))
        {
            Vector3 objectDir = transform.forward;
            Vector3 otherNormal = other.GetContact(0).normal;

            _rb.velocity = Vector3.Reflect(_rb.velocity, otherNormal);

            _rb.angularVelocity = -_rb.angularVelocity;
        }
    }

    private void RoundSpeed()
    {
        if (_rb.velocity.sqrMagnitude <= 0.002f)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    private void PlayCollisionSound(float volume)
    {
        _audioSourse.PlayOneShot(_audioSourse.clip, volume);
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
