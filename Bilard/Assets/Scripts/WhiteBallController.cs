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
    private bool areBallsMoving = false;
    private Coroutine ballMovingCoroutine;
    private Camera mainCam;
    private GameController _gameController;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphColl = GetComponent<SphereCollider>();
        pool.transform.position = transform.position - new Vector3(0,0,0.01f);
        mainCam = Camera.main;
        _gameController = GameController.instance;
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.anyKeyDown)
        {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
        }
        isFoul = _gameController.IsFoul();
        if(isFoul && !areBallsMoving)
        {
            FoulState();
        }
        currentYaw -= Input.GetAxisRaw("Horizontal") * yawSpeed * Time.deltaTime;
        
    }
    private void LateUpdate() {
        if(Input.GetKeyDown(KeyCode.Space) && !_gameController.IsFoul() && Time.timeScale != 0 && !areBallsMoving)
        {
            Shoot();
        }
        ManageRotation();
        
    }
    private void ManageRotation()
    {
        pool.transform.position = transform.position - new Vector3(0,0,0.1f);
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
        pool.SetActive(false);
        // Think about implementation
        //StartCoroutine(_gameController.AreBallsMovingEnumerator());
        StartCoroutine(WaitForBallsToStop());
    }
    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, transform.position+shotForce);
    }
    public void FoulState()
    {
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        sphColl.enabled = false;
        if(Time.timeScale != 0)
        {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, 100f, _gameController.WhatIsTable()))
            transform.position = hit.point;
        if(Input.GetMouseButtonDown(0))
        {
            _gameController.EndFoul();
            rb.useGravity = true;
            rb.isKinematic = false;
            sphColl.enabled = true;
        }
        }
    }
    // TO DO : POOL DISAPPEARING/REAPPEARING AND MAKE BALLS STANDING TRUE AT THE END
    public IEnumerator WaitForBallsToStop()
    {
        areBallsMoving = true;
        yield return new WaitForSeconds(0.1f);
        while(_gameController.AreBallsMoving())
        {
            yield return new WaitForSeconds(0.15f);
        }
        areBallsMoving = false;
        pool.SetActive(true);
        _gameController.CheckChangeTurn();
    }
    private void OnCollisionEnter(Collision other) {
        BallController ballController = other.gameObject.GetComponent<BallController>();
        if(ballController != null)
        {
            BallType otherBallType = ballController.getBallType();
            _gameController.OnWhiteBallFirstHit(this, otherBallType);
        }
    }
}
