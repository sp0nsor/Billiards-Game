using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StrikeBallView : MonoBehaviour
{
    public Slider Slider;
    public LayerMask TableLayer;
    public Image ShotPowerSliderFill;
    public GameObject Stick;

    private bool _isTakingShot;

    public event Action<Vector3> OnTouchDetected;
    public static event Action<float, float> OnShotEnded;

    public event Action<float, float> OnSliderValueChanged;

    private const float MaxShotPower = 100f;
    private const float MinDistanceFromBall = 0.1f;

    private void Awake()
    {
        Slider.onValueChanged.AddListener(HandleSliderValueChanged);
        StrikeBallController.OnVisibilityStatusChange += ABC;
    }

    private void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            
            if(Physics.Raycast(ray, out RaycastHit hit, 100f, TableLayer))
                if(touch.phase == TouchPhase.Moved)
                    OnTouchDetected?.Invoke(hit.point);
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

        OnShotEnded?.Invoke(Slider.value, Stick.transform.position.y);
        SetDefaultSliderValue();

        _isTakingShot = false;
    }

    public void RotateStickAroundBall(Vector3 ballPosition, float currentYaw)
    {
        Stick.transform.RotateAround(ballPosition, Vector3.up, currentYaw);
        Stick.transform.LookAt(ballPosition);
    }

    public void SetStickPosition(Vector3 ballPosition)
    {
        float distanceFromBall = Slider.value / MaxShotPower + MinDistanceFromBall;
        Stick.transform.position = ballPosition - new Vector3(0, 0, distanceFromBall);
    }

    private void SetDefaultSliderValue()
    {
        Slider.onValueChanged.RemoveListener(HandleSliderValueChanged);
        Slider.value = Slider.minValue;
        Slider.onValueChanged.AddListener(HandleSliderValueChanged);
    }

    private void HandleSliderValueChanged(float value)
    {
        if (!_isTakingShot)
            StartCoroutine(HandleShotPower());
    }

    private void ABC(bool status, Vector3 ballPosition)
    {
        if(status)
            Stick.SetActive(true);
        else
            Stick.SetActive(false);

        SetStickPosition(ballPosition);
        RotateStickAroundBall(ballPosition, 0f);
    }

    private void OnDestroy()
    {
        Slider.onValueChanged.RemoveListener(HandleSliderValueChanged);
    }
}
