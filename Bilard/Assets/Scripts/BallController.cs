using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum BallType {NULL, WHITE, HALF, FULL, BLACK};
[RequireComponent(typeof(SphereCollider))]
public class BallController : MonoBehaviour
{
    
    private Rigidbody _rb;
    [SerializeField] private BallType ballType;
    [SerializeField] private int ballNumber;
    // Start is called before the first frame update
    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        PhysicsController.physicsDelegate += ApplyPhysics;
    }
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Band")
        {
        Vector3 objectDir = transform.forward;
        Vector3 otherNormal = other.contacts[0].normal;
        
        _rb.velocity = Vector3.Reflect(_rb.velocity, otherNormal);
        _rb.angularVelocity = -_rb.angularVelocity;
        }
    }
    public void ApplyPhysics()
    {
        if(_rb != null)
        {
            _rb.angularDrag = PhysicsController.instance.getAngularDrag();
            _rb.mass = PhysicsController.instance.getBallMass();
            _rb.drag = PhysicsController.instance.getDrag();
        }
    }
    public void GotPocketed(PocketType pocketType)
    {
        GameController.instance.CheckPocketedBall(this, pocketType);
        if(ballType == BallType.HALF || ballType == BallType.FULL)
        {
        PhysicsController.physicsDelegate -= ApplyPhysics;
        Destroy(gameObject, 2f);
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
        return _rb.velocity != Vector3.zero;
    }
}
