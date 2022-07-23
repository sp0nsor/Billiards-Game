using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WhiteBallController : MonoBehaviour
{
    public GameObject pool;
    [SerializeField]
    private Slider slider;
    private LineRenderer lineRenderer;
    private Rigidbody rb;
    private SphereCollider sphColl;
    private bool timeToShoot = true;
    [SerializeField] private float yawSpeed;
    [SerializeField] private float currentYaw = 0f, yawSpeedPlus = 0f, horizontalAxis;
    private Vector3 shotForce = Vector3.forward * 2;
    private float shotAngle, shotPower = 1;
    private bool isFoul = false, hitBall = false, pressingButton;
    private bool areBallsMoving = false;
    private Coroutine ballMovingCoroutine;
    private Camera mainCam;
    private GameController _gameController;
    private UIManager _uiManager;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphColl = GetComponent<SphereCollider>();
        _uiManager = FindObjectOfType<UIManager>();
        pool.transform.position = transform.position - new Vector3(0, 0, 0.01f);
        mainCam = Camera.main;
        _gameController = GameController.instance;
        lineRenderer = FindObjectOfType<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!areBallsMoving)
        {
            isFoul = _gameController.IsFoul();
            if (isFoul)
            {
                FoulState();
                return;
            }
            ManageLine();
        }
    }
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_gameController.IsFoul() && Time.timeScale != 0 && !areBallsMoving)
        {
            StartCoroutine(HandleShotPower());
        }
        ManageRotation();

    }
    private void ManageRotation()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        if (horizontalAxis != 0)
        {
            currentYaw -= Input.GetAxisRaw("Horizontal") * (yawSpeed + yawSpeedPlus) * Time.deltaTime;
            yawSpeedPlus += 20 * Time.deltaTime;
        }
        else
        {
            yawSpeedPlus = 0;
        }
        pool.transform.position = transform.position - new Vector3(0, 0, 0.1f);
        pool.transform.RotateAround(transform.position, Vector3.up, currentYaw);
        pool.transform.LookAt(transform.position);

        shotAngle = pool.transform.eulerAngles.y;
        shotForce = Quaternion.Euler(0, shotAngle, 0) * new Vector3(0, 0, 3);
    }
    private void ManageLine()
    {
        lineRenderer.enabled = true;
        RaycastHit raycastHit;
        Physics.Raycast(transform.position, shotForce, out raycastHit);
        Vector3 startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z),
        targetPosition = new Vector3(raycastHit.point.x, raycastHit.point.y, raycastHit.point.z);
        Vector3 tempVec = targetPosition - startPosition;
        float tang = tempVec.z / tempVec.x;
        float angle = Mathf.Atan(tang);
        float angleDegrees = angle*Mathf.Rad2Deg;
        Debug.Log(angleDegrees);
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, targetPosition);
    }
    private IEnumerator HandleShotPower()
    {
        _uiManager.EnableShotSlider(transform.position);
        yield return new WaitForSeconds(0.03f);
        while (true)
        {
            if (!Input.GetKey(KeyCode.Space))
                break;
            if (shotPower < 100)
            {
                shotPower += 1;
                _uiManager.UpdateShotSlider(shotPower);
            }
            yield return new WaitForSeconds(0.03f);
        }
        _uiManager.DisableShotSlider();
        lineRenderer.enabled = false;
        Shoot();
        shotPower = 1;

    }
    private void Shoot()
    {
        float degree = pool.transform.rotation.y;
        //Vector3 forceV = new Vector3(Mathf.Sin(degree)*1, 0, Mathf.Cos(degree)*1);
        shotForce = Quaternion.Euler(0, shotAngle, 0) * new Vector3(0, 0, shotPower / 10 * 0.9f);
        rb.AddForce(shotForce, ForceMode.Impulse);
        pool.SetActive(false);
        StartCoroutine(WaitForBallsToStop());
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + shotForce);
    }
    // BUG after foul ball falls through table
    public void FoulState()
    {
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        sphColl.enabled = false;
        pool.SetActive(false);
        if (Time.timeScale != 0)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _gameController.WhatIsTable()))
                transform.position = hit.point;
            if (Input.GetMouseButtonDown(0))
            {
                _gameController.EndFoul();
                rb.useGravity = true;
                rb.isKinematic = false;
                sphColl.enabled = true;
                pool.SetActive(true);
            }
        }
    }
    public IEnumerator WaitForBallsToStop()
    {
        areBallsMoving = true;
        yield return new WaitForSeconds(0.1f);
        while (_gameController.AreBallsMoving())
        {
            yield return new WaitForSeconds(0.15f);
        }
        areBallsMoving = false;
        if (!hitBall)
            _gameController.Foul();
        pool.SetActive(true);
        _gameController.CheckChangeTurn();
        hitBall = false;
    }
    private void OnCollisionEnter(Collision other)
    {
        BallController ballController = other.gameObject.GetComponent<BallController>();
        if (!hitBall && ballController != null)
        {
            BallType otherBallType = ballController.getBallType();
            _gameController.OnWhiteBallFirstHit(this, otherBallType);
            hitBall = true;
        }
    }
}
