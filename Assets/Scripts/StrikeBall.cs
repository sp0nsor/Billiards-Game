using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class StrikeBall : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private Slider slider;
    public GameObject stick;
    private Rigidbody _rb;
    private float _currentYaw = 0f;
    private Vector3 _shotForce = Vector3.forward * 2;
    private float _shotAngle, _shotPower = 1;
    private bool _isTakingShot = false;
    private Camera _mainCam;
    private GameController _gameController;
    private Coroutine _handleShotPowerCoroutine = null;
    private readonly WaitForSeconds _shotPoweringUpTime = new(0.03f);
    private static StrikeBall _currentActiveBall;
    private static bool _firstMove = true;
    private BallController _ballController;
    private Transform _cachedTransform;
    private Transform _stickTransform;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _mainCam = Camera.main;
        _gameController = GameController.Instance;
        _ballController = GetComponent<BallController>();
        _cachedTransform = transform;
        _stickTransform = stick.transform;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Ray ray = _mainCam.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, _gameController.WhatIsTable()))
                {
                    _currentYaw = CalculateDegree(_cachedTransform.position, hit.point);
                }
            }
        }
        if (slider.value > 1 && _handleShotPowerCoroutine == null && _isTakingShot == false) { _handleShotPowerCoroutine = StartCoroutine(HandleShotPower()); }
        if (slider.value == 1 && _handleShotPowerCoroutine != null) { FinishShotHandling(); }

        ManageRotation();
        ManageLine();
    }

    private void FinishShotHandling()
    {
        StopCoroutine(_handleShotPowerCoroutine);
        _handleShotPowerCoroutine = null;
        _shotPower = 1;
        _uiManager.DisableShotSlider();
        _isTakingShot = false;
    }

    private void ManageRotation()
    {
        if (!_isTakingShot)
        {
            RotateStickAroundBall();
        }

        _shotAngle = _stickTransform.eulerAngles.y;
        _shotForce = Quaternion.Euler(0, _shotAngle, 0) * new Vector3(0, 0, _shotPower / 10 * 0.9f);
    }

    private void RotateStickAroundBall()
    {
        float distanceFromBall = 0.1f + (0.2f * _shotPower / 90);
        var position = _cachedTransform.position;
        _stickTransform.position = position - new Vector3(0, 0, distanceFromBall);
        _stickTransform.RotateAround(position, Vector3.up, _currentYaw);
        _stickTransform.LookAt(position);
    }

    private void ManageLine()
    {
        _lineRenderer.enabled = true;
        RaycastHit raycastHit;
        var position = _cachedTransform.position;
        Physics.Raycast(position, _shotForce, out raycastHit);
        Vector3 startPosition = position,
        targetPosition = raycastHit.point;
        _lineRenderer.SetPosition(0, startPosition);
        _lineRenderer.SetPosition(1, targetPosition);
    }

    private IEnumerator HandleShotPower()
    {
        _isTakingShot = true;
        _uiManager.EnableShotSlider(transform.position);
        yield return _shotPoweringUpTime;

        while (true)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
                break;

            _shotPower = slider.value;
            _uiManager.UpdateShotSlider(_shotPower);
            RotateStickAroundBall();
            
            yield return _shotPoweringUpTime;
        }
        Shoot();
        DisableControlsAfterShot();
        DisableController();
        GameController.Instance.StartCoroutine(GameController.Instance.WaitForBallsToStop());
    }

    private void DisableControlsAfterShot()
    {
        stick.SetActive(false);
        _uiManager.DisableShotSlider();
        _lineRenderer.enabled = false;
        _firstMove = false;
        _isTakingShot = false;
        _shotPower = 1;
        slider.value = 1;
    }

    private void Shoot()
    {
        _shotForce = Quaternion.Euler(0, _shotAngle, 0) * new Vector3(0, 0, _shotPower / 10 * 0.9f);
        _rb.AddForce(_shotForce, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(_cachedTransform.position, _cachedTransform.position + _shotForce);
    }

    private float CalculateDegree(Vector3 from, Vector3 to)
    {
        Vector3 vectorBetween = to - from;
        float degree = Mathf.Atan2(vectorBetween.x, vectorBetween.z) * Mathf.Rad2Deg;

        return (degree + 360f) % 360f;
    }

    public static void SetCurrentActiveBall(StrikeBall ball)
    {
        _currentActiveBall = ball;
    }

    public static void SetFirstMove(bool value)
    {
        _firstMove = value;
    }

    public BallController GetBallController()
    {
        return _ballController;
    }

    public static StrikeBall CurrentActiveBall
    {
        get { return _currentActiveBall; }
    }

    public static bool FirstMove
    {
        get { return _firstMove; }
    }

    public void EnabledController()
    {
        enabled = true;
    }

    public void DisableController()
    {
        enabled = false;
    }

    public void EnabledStick()
    {
        stick.SetActive(true);
    }
}
