using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WhiteBallController : MonoBehaviour
{
    public GameObject stick;
    [SerializeField] private Slider slider;
    private LineRenderer lineRenderer;
    private Rigidbody rb;
    private SphereCollider sphColl;
    private bool timeToShoot = true;
    [SerializeField] private float yawSpeed;
    [SerializeField] private float currentYaw = 0f, yawSpeedPlus = 0f, horizontalAxis;
    private Vector3 shotForce = Vector3.forward * 2;
    private float shotAngle, shotPower = 1;
    [SerializeField] private bool /*isFoul = false,*/ hitBall = false, areBallsMoving = false, stickHit = false, isTakingShot = false, triangleBroken = false, isControllerEnabled = true;
    private Coroutine ballMovingCoroutine;
    private Camera mainCam;
    private GameController _gameController;
    private UIManager _uiManager;
    private Coroutine handleShotPowerCoroutine;
    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    private WaitForSeconds shotPoweringUpTime = new WaitForSeconds(0.03f), waitForBallsStopTime = new WaitForSeconds(0.15f);
    private static WhiteBallController currentActiveBall;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphColl = GetComponent<SphereCollider>();
        _uiManager = FindObjectOfType<UIManager>();
        mainCam = Camera.main;
        _gameController = GameController.instance;
        lineRenderer = FindObjectOfType<LineRenderer>();
        stickHit = false;
    }

    void Update()
    {
        if (!areBallsMoving)
        {
            if (Input.GetMouseButtonDown(0) && Time.timeScale != 0)
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, _gameController.WhatIsTable()))
                    currentYaw = CalculateDegree(transform.position, hit.point);
            }
            ManageLine();
        }
    }
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_gameController.IsFoul() && Time.timeScale != 0 && !areBallsMoving)
        {
            handleShotPowerCoroutine = StartCoroutine(HandleShotPower());
            //StartCoroutine(HandleShotPower());
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) && isTakingShot && Time.timeScale != 0)
        {
            StopCoroutine(handleShotPowerCoroutine);
            handleShotPowerCoroutine = null;
            shotPower = 1;
            _uiManager.DisableShotSlider();
            isTakingShot = false;
        }

        ManageRotation();

    }

    // method that manages rotation of stick and force that will be used to shoot ball
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
        float distanceFromBall = 0.1f + (0.2f * shotPower / 100);
        if (!isTakingShot)
        {
            stick.transform.position = transform.position - new Vector3(0, 0, distanceFromBall);
            stick.transform.RotateAround(transform.position, Vector3.up, currentYaw);
            stick.transform.LookAt(transform.position);
        }


        shotAngle = stick.transform.eulerAngles.y;
        shotForce = Quaternion.Euler(0, shotAngle, 0) * new Vector3(0, 0, shotPower / 10 * 0.9f);
    }
    // method that manages line being drawn by Line Renderer
    private void ManageLine()
    {
        lineRenderer.enabled = true;
        RaycastHit raycastHit;
        Physics.Raycast(transform.position, shotForce, out raycastHit);
        Vector3 startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z),
        targetPosition = new Vector3(raycastHit.point.x, raycastHit.point.y, raycastHit.point.z);
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, targetPosition);
    }
    // As name says its a Coroutine that increases shot power by 1 every 30ms until spacebar is no longer pressed, then it calls Shoot method
    private IEnumerator HandleShotPower()
    {
        isTakingShot = true;
        _uiManager.EnableShotSlider(transform.position);
        yield return shotPoweringUpTime;
        while (true)
        {
            if (!Input.GetKey(KeyCode.Space))
                break;
            if (shotPower < 100)
            {
                shotPower += 1;
                _uiManager.UpdateShotSlider(shotPower);
                float distanceFromBall = 0.1f + (0.2f * shotPower / 100);

                stick.transform.position = transform.position - new Vector3(0, 0, distanceFromBall);
                stick.transform.RotateAround(transform.position, Vector3.up, currentYaw);
                stick.transform.LookAt(transform.position);
            }
            yield return shotPoweringUpTime;
        }
        shotForce = Quaternion.Euler(0, shotAngle, 0) * new Vector3(0, 0, shotPower / 10 * 0.9f);
        while (!stickHit)
        {
            Debug.Log(shotForce);
            stick.transform.position += shotForce * Time.deltaTime;
            yield return waitForEndOfFrame;
        }
        stick.SetActive(false);
        stickHit = false;
        _uiManager.DisableShotSlider();
        lineRenderer.enabled = false;
        Shoot();
        isTakingShot = false;
        shotPower = 1;

    }
    // Method that shoots white ball with force calculated from current rotation of stick
    private void Shoot()
    {
        //Vector3 forceV = new Vector3(Mathf.Sin(degree)*1, 0, Mathf.Cos(degree)*1);
        shotForce = Quaternion.Euler(0, shotAngle, 0) * new Vector3(0, 0, shotPower / 10 * 0.9f);
        rb.AddForce(shotForce, ForceMode.Impulse);
        DisableController();
        StartCoroutine(WaitForBallsToStop());
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + shotForce);
    }
    // BUG after foul ball falls through table
    /*public void FoulState()
    {
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        sphColl.enabled = false;
        stick.SetActive(false);
        if (Time.timeScale != 0)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _gameController.WhatIsTable()))
                transform.position = new Vector3(hit.point.x, hit.point.y + 0.04f, hit.point.z);
            if (Input.GetMouseButtonDown(0))
            {
                _gameController.EndFoul();
                rb.useGravity = true;
                rb.isKinematic = false;
                sphColl.enabled = true;
                stick.SetActive(true);
            }
        }
    }*/
    // Method that calculates degrees in Y-axis considering Camera's WorldSpace
    public float CalculateDegree(Vector3 from, Vector3 to)
    {
        float degree, tang, result;
        Vector3 vectorBetween = to - from;
        tang = vectorBetween.x / vectorBetween.z;
        degree = Mathf.Atan(tang);
        if (vectorBetween.x > 0 && vectorBetween.z > 0)
        {
            result = degree * Mathf.Rad2Deg;
            return result;
        }
        if (vectorBetween.x > 0 && vectorBetween.z < 0)
        {
            result = 90 + degree * Mathf.Rad2Deg + 90;
            return result;
        }
        if (vectorBetween.x < 0 && vectorBetween.z < 0)
        {
            result = 180 + degree * Mathf.Rad2Deg;
            return result;
        }
        result = 270 + degree * Mathf.Rad2Deg + 90;
        return result;
    }
    public IEnumerator WaitForBallsToStop()
    {
        areBallsMoving = true;
        yield return waitForBallsStopTime;
        while (_gameController.AreBallsMoving())
        {
            yield return waitForBallsStopTime;
        }
        areBallsMoving = false;
        if (!hitBall)
            _gameController.Foul();
        stick.SetActive(true);
        _gameController.CheckChangeTurn();
        hitBall = false;
    }
    private void OnCollisionEnter(Collision other)
    {
        BallController ballController = other.gameObject.GetComponent<BallController>();
        if (!hitBall && ballController != null)
        {
            /*BallType otherBallType = ballController.getBallType();
            _gameController.OnWhiteBallFirstHit(this, otherBallType);*/
            hitBall = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Stick")
        {
            stickHit = true;
        }
    }
    public static void SetCurrentActiveBall(WhiteBallController ball)
    {
        currentActiveBall = ball;
    }
    public static WhiteBallController CurrentActiveBall
    {
        get { return currentActiveBall; }
    }
    public void EnabledController()
    {
        enabled = true;
    }
    public void DisableController()
    {
        enabled = false;
    }
    public bool IsControllerEnabled()
    {
        return enabled;
    }
}