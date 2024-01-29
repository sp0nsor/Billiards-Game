using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    [FormerlySerializedAs("Ball2DSprites")] [SerializeField] private Sprite[] ball2DSprites = new Sprite[15];
    [FormerlySerializedAs("powerSliderGO")] [SerializeField] private GameObject powerSliderGo;
    [SerializeField] private TextMeshProUGUI massText, dragText, angDragText, endMessageText;
    [SerializeField] private Slider massSlider, dragSlider, angDragSlider, shotPowerSlider;
    [SerializeField] private GameObject[] player1Balls, player2Balls;
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject endMessage;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private Image shotPowerSliderFill;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private Camera mainCam, uiCam;
    [SerializeField] private Color fillColor;

    private void Start()
    {
        SetupUI();
        SetUpSliders();
        UpdateSliders();
        mainCam = Camera.main;
    }

    public void ShowGameMenu()
    {
        if (!gameMenu.activeSelf && !settings.activeSelf)
        {
            gameMenu.SetActive(!gameMenu.activeSelf);
            Time.timeScale = 0;
        }
        else
        {
            DisableMenus();
        }
    }

    public void DisableMenus()
    {
        gameMenu.SetActive(false);
        settings.SetActive(false);
        Time.timeScale = 1;
    }

    public void SetUpSliders()
    {
        massSlider.value = PhysicsController.Instance.GetDefaultBallMass();
        massSlider.onValueChanged.AddListener((v) =>
        {
            PhysicsController.Instance.SetTempMass(v);
            if (v.ToString().Length > 4)
                massText.text = v.ToString().Substring(0, 4);
            else
                massText.text = v.ToString();
        });
        dragSlider.value = PhysicsController.Instance.GetDefaultDrag();
        dragSlider.onValueChanged.AddListener((v) =>
        {
            PhysicsController.Instance.SetTempDrag(v);
            if (v.ToString().Length > 4)
                dragText.text = v.ToString().Substring(0, 4);
            else
                dragText.text = v.ToString();
        });
        angDragSlider.value = PhysicsController.Instance.GetDefaultAngularDrag();
        angDragSlider.onValueChanged.AddListener((v) =>
        {
            PhysicsController.Instance.SetTempAngularDrag(v);
            if (v.ToString().Length > 4)
                angDragText.text = v.ToString().Substring(0, 4);
            else
                angDragText.text = v.ToString();
        });
    }

    public void UpdateSliders()
    {
        massSlider.value = PhysicsController.Instance.GetBallMass();
        string temp = PhysicsController.Instance.GetBallMass().ToString();
        if (temp.Length > 4)
            temp = temp.Substring(0, 4);
        massText.text = temp;
        dragSlider.value = PhysicsController.Instance.GetDrag();
        temp = PhysicsController.Instance.GetDrag().ToString();
        if (temp.Length > 4)
            temp = temp.Substring(0, 4);
        dragText.text = temp;
        angDragSlider.value = PhysicsController.Instance.GetAngularDrag();
        temp = PhysicsController.Instance.GetAngularDrag().ToString();
        if (temp.Length > 4)
            temp = temp.Substring(0, 4);
        angDragText.text = temp;
    }

    public void ApplyPhysicsButton()
    {
        PhysicsController.Instance.ApplyPhysicsChanges();
        PhysicsController.PhysicsDelegate.Invoke();
    }

    public void OnDefaultPhysicsButton()
    {
        PhysicsController.Instance.SetDefaultPhysics();
        UpdateSliders();
        PhysicsController.PhysicsDelegate.Invoke();
    }

    public void OnMenuCloseButton(string menuName)
    {
        string temp = string.Concat(menuName.Where(c => !char.IsWhiteSpace(c)));
        temp = temp.ToLower();
        if (temp.Contains("main"))
        {
            DisableMenus();
        }
        if (temp.Contains("settings"))
        {
            settings.SetActive(false);
            gameMenu.SetActive(true);
        }
    }

    public void OnResumeButtonClick()
    {
        DisableMenus();
    }

    public void OnSettingsButtonClick()
    {
        gameMenu.SetActive(false);
        UpdateSliders();
        settings.SetActive(true);
    }

    public void OnMainMenuButtonClick()
    {
        StrikeBall.SetFirstMove(true);
        Time.timeScale = 1;
        SceneManager.LoadScene("Main menu");
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }
    
    public void EnableShotSlider(Vector3 pos)
    {
        var screen = mainCam.WorldToScreenPoint(pos + new Vector3(0.10f, 0, -0.055f));
        screen.z = (canvas.transform.position - uiCam.transform.position).magnitude;
        var position = uiCam.ScreenToWorldPoint(screen);
        shotPowerSlider.transform.position = position;
        powerSliderGo.SetActive(true);
        shotPowerSlider.enabled = true;
    }
    
    public void DisableShotSlider()
    {
        powerSliderGo.SetActive(false);
        shotPowerSlider.enabled = false;
        shotPowerSlider.value = 1;
    }
    
    public void UpdateShotSlider(float shotPower)
    {
        shotPowerSlider.value = shotPower + 5;
        float red, green, blue;
        red = 5.1f * shotPower + 5;
        green = 255 - 5.1f * (shotPower);
        blue = 0;
        red = Mathf.Clamp(red, 0, 255);
        green = Mathf.Clamp(green, 0, 255);
        fillColor = new Color32((byte)red, (byte)green, (byte)blue, 255);
        shotPowerSliderFill.color = fillColor;
    }
    
    public void SetupUI()
    {
        for (int i = 0; i < player1Balls.Count(); i++)
        {
            Image ballImage = player1Balls[i].GetComponent<Image>();
            ballImage.enabled = false;
        }
        for (int i = 0; i < player2Balls.Count(); i++)
        {
            Image ballImage = player2Balls[i].GetComponent<Image>();
            ballImage.enabled = false;
        }
        turnText.text = GameController.Instance.GetGameState() == GameState.Player1Turn ? "P1 TURN" : "P2 TURN";
    }
    
    public void UpdateUI(List<int> PocketedBallsP1, List<int> PocketedBallsP2)
    {
        turnText.text = GameController.Instance.GetGameState() == GameState.Player1Turn ? "P1 TURN" : "P2 TURN";
        for (int i = 0; i < PocketedBallsP1.Count; i++)
        {
            Image ballImage = player1Balls[i].GetComponent<Image>();
            ballImage.sprite = ball2DSprites[PocketedBallsP1[i]];
            ballImage.enabled = true;
        }
        for (int i = 0; i < PocketedBallsP2.Count; i++)
        {
            Image ballImage = player2Balls[i].GetComponent<Image>();
            ballImage.sprite = ball2DSprites[PocketedBallsP2[i]];
            ballImage.enabled = true;

        }
    }
    
    public void OnGameEnd(string message)
    {
        endMessageText.text = message;
        endMessage.SetActive(true);
        Time.timeScale = 0;
    }
    
    public void OnPlayAgainButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
