using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBallController : MonoBehaviour
{
    public GameObject pool;
    private Rigidbody rb;
    private SphereCollider sphColl;
    private bool timeToShoot = true;
    public float yawSpeed = 100f;
    private float currentYaw = 0f;
    private Vector3 shotForce = Vector3.forward*2;
    private float shotAngle;
    private float ratio;
    private bool isFoul = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphColl = GetComponent<SphereCollider>();
        pool.transform.position = transform.position - new Vector3(0,0,0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        pool.transform.position = transform.position - new Vector3(0,0,0.2f);
        if(Input.anyKeyDown)
        {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
        }
        isFoul = GameController.instance.IsFoul();
        if(isFoul)
        {
            FoulState();
        }
        currentYaw -= Input.GetAxisRaw("Horizontal") * yawSpeed * Time.deltaTime;
        
    }
    private void LateUpdate() {
        if(Input.GetKeyDown(KeyCode.Space) && !GameController.instance.IsFoul() && Time.timeScale != 0)
        {
            Shoot();
        }
        ManageRotation();
        
    }
    private void ManageRotation()
    {
        pool.transform.RotateAround(transform.position,Vector3.up, currentYaw);
        pool.transform.LookAt(transform.position);
        shotAngle = pool.transform.eulerAngles.y;
        shotForce = Quaternion.Euler(0, shotAngle, 0) * new Vector3(0,0,3); 
    }
    private void Shoot()
    {
        float degree = pool.transform.rotation.y;
        Vector3 forceV = new Vector3(Mathf.Sin(degree)*1, 0, Mathf.Cos(degree)*1);
        rb.AddForce(shotForce, ForceMode.Impulse);
    }
    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, transform.position+shotForce);
    }
    public void FoulState()
    {
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        sphColl.enabled = false;
        if(Time.timeScale != 0)
        {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, 100f, GameController.instance.WhatIsTable()))
            transform.position = hit.point;
        if(Input.GetMouseButtonDown(0))
        {
            GameController.instance.EndFoul();
            rb.useGravity = true;
            sphColl.enabled = true;
        }
        }
    }
}
