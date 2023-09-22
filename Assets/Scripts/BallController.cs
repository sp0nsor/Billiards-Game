using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public enum BallType { BLACK, WHITE };
[RequireComponent(typeof(SphereCollider))]
public class BallController : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private BallType ballType;
    [SerializeField] private int ballNumber;
    [SerializeField] private AudioSource audioSource;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        PhysicsController.physicsDelegate += ApplyPhysics;
        
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Band")
        {
            Vector3 objectDir = transform.forward;
            Vector3 otherNormal = other.GetContact(0).normal;

            _rb.velocity = Vector3.Reflect(_rb.velocity, otherNormal);

            _rb.angularVelocity = -_rb.angularVelocity;
        }
        // if (other.gameObject.tag == "Ball")
        // {
        //     audioSource.Play();
        // }
    }
    public void ManageVelocity()
    {
        _rb.velocity = _rb.velocity * 0.9985f;
        _rb.angularVelocity = _rb.angularVelocity * 0.9985f;
        if (ballType == BallType.WHITE) { Debug.Log(_rb.velocity); }
    }
    public IEnumerator ManageVelocityEnum()
    {
        while (true)
        {
            _rb.velocity = _rb.velocity * 0.991f;
            _rb.angularVelocity = _rb.angularVelocity * 0.991f;
            yield return new WaitForFixedUpdate();
        }
    }
    public void ApplyPhysics()
    {
        if (_rb != null)
        {
            _rb.angularDrag = PhysicsController.instance.getAngularDrag();
            _rb.mass = PhysicsController.instance.getBallMass();
            _rb.drag = PhysicsController.instance.getDrag();
        }
    }
    public void GotPocketed()
    {
        GameController.instance.CheckPocketedBall(this);
        if (ballType == BallType.BLACK || ballType == BallType.WHITE)
        {
            PhysicsController.physicsDelegate -= ApplyPhysics;
            Destroy(gameObject, 1f);
        }
    }
    public BallType getBallType()
    {
        return ballType;
    }
    public int getBallNumber()
    {
        return ballNumber;
    }
    public bool isMoving()
    {
        return _rb.velocity.magnitude != 0;
    }
}