using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject settings;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowGameMenu()
    {
        gameMenu.SetActive(!gameMenu.activeSelf);
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }
    public void DisableMenus()
    {
        gameMenu.SetActive(false);
        settings.SetActive(false);
        Time.timeScale = 1;
    }
    public void ApplyPhysicsButton()
    {
        PhysicsController.physicsDelegate.Invoke();
    }
    public void onMenuCloseButton(string menuName)
    {
        string temp = string.Concat(menuName.Where(c => !char.IsWhiteSpace(c)));
        temp = temp.ToLower();
        if(temp.Contains("main"))
        {
            Time.timeScale = 1;
            gameMenu.SetActive(false);
        }
        if(temp.Contains("settings"))
        {
            settings.SetActive(false);
            gameMenu.SetActive(true);
        }
    }
    public void onResumeButtonClick()
    {
        DisableMenus();
    }
    public void onSettingsButtonClick()
    {
        gameMenu.SetActive(false);
        settings.SetActive(true);
    }
    public void onMainMenuButtonClick()
    {
        // goes back to main menu
    }
    public void onExitButtonClick()
    {
        //exit game
        Application.Quit();
    }
}
