using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameOver;

    [Header("Main Menu")]
    public string mainMenuSceneName = "Main Menu";

    private bool isGameOverShown = false;

    void Awake()
    {
        Time.timeScale = 1f;

        if (gameOver != null)
        {
            gameOver.SetActive(false);
        }
    }

    public void ShowGameOver()
    {
        if (isGameOverShown)
        {
            return;
        }

        isGameOverShown = true;

        if (gameOver != null)
        {
            gameOver.SetActive(true);
            gameOver.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogError("GameOver panel is not assigned.");
        }

        Time.timeScale = 0f;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void UnPause()
    {
        Time.timeScale = 1f;
    }
}