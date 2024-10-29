using UnityEngine;
using UnityEngine.SceneManagement;

public class FrameLimiter : MonoBehaviour
{
    [SerializeField] private GameObject gameMenu;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    public void OnPause()
    {
        gameMenu.SetActive(true);
        Time.timeScale = 0f;
    }
    public void OnResume()
    {
        gameMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnExit()
    {
        Application.Quit();
    }
    public void OnPlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
