using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameView : MonoBehaviour, IGameView
{
    [SerializeField] private Slider Slider;
    [SerializeField] private LayerMask TableLayer;
    [SerializeField] private GameObject Stick;
    [SerializeField] private GameController GameController;

    private bool _isTakingShot;
    private BallController CurrentBall;

    private const float MaxShotPower = 100f;
    private const float MinDistanceFromBall = 0.1f;

    private void Awake()
    {
        Slider.onValueChanged.AddListener(HandleSliderValueChanged);
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, TableLayer))
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    float currentYaw = CalculateDegree(hit.point, CurrentBall.transform.position);
                    SetStickPosition(CurrentBall.transform.position);
                    RotateStickAroundBall(CurrentBall.transform.position, currentYaw);
                }
            }

        }
    }

    private IEnumerator HandleShotPower()
    {
        _isTakingShot = true;

        while (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended && Slider.value == Slider.minValue)
            {
                _isTakingShot = false;
                yield break;
            }

            yield return null;
        }
        CurrentBall.Shot(Slider.value, Stick.transform.eulerAngles.y);
        GameController.StartWaitForBallsToStop();
        SetDefaultSliderValue();

        _isTakingShot = false;
    }

    private float CalculateDegree(Vector3 to, Vector3 from)
    {
        Vector3 vectorBetween = to - from;
        float degree = Mathf.Atan2(vectorBetween.x, vectorBetween.z) * Mathf.Rad2Deg;

        return  (degree + 360f) % 360f;
    }

    private void RotateStickAroundBall(Vector3 ballPosition, float currentYaw)
    {
        Stick.transform.RotateAround(ballPosition, Vector3.up, currentYaw);
        Stick.transform.LookAt(ballPosition);
    }

    private void SetStickPosition(Vector3 ballPosition)
    {
        float distanceFromBall = Slider.value / MaxShotPower + MinDistanceFromBall;
        Stick.transform.position = ballPosition - new Vector3(0, 0, distanceFromBall);
    }

    private void SetDefaultSliderValue()
    {
        Slider.onValueChanged.RemoveListener(HandleSliderValueChanged);
        Slider.value = Slider.minValue;
        Slider.onValueChanged.AddListener(HandleSliderValueChanged);
        Stick.SetActive(false);
    }           

    private void HandleSliderValueChanged(float value)
    {
        if (!_isTakingShot)
            StartCoroutine(HandleShotPower());
    }


    public void UpdateCurrentBall(BallController ballController)
    {
        CurrentBall = ballController;

        Stick.SetActive(true);
        SetStickPosition(CurrentBall.transform.position);
        RotateStickAroundBall(CurrentBall.transform.position, 0f);
    }

    private void OnDestroy()
    {
        Slider.onValueChanged.RemoveListener(HandleSliderValueChanged);
    }
}
