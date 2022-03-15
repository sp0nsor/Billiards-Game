using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //_rb.velocity = _rb.velocity*0.99f;
    }
    private void FixedUpdate() {
        //_rb.AddForce(Vector3.down, ForceMode.VelocityChange);
    }
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Band")
        {
        Vector3 objectDir = transform.forward;
        Vector3 otherNormal = other.contacts[0].normal;
        _rb.velocity = Vector3.Reflect(_rb.velocity, otherNormal);
        //_rb.AddTorque(_rb.velocity);
        }
    }
}
