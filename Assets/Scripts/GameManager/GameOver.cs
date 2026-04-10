using UnityEngine;

public class GameOver : MonoBehaviour
{
    public GameObject gameOver;

    void Awake()
    {
        if (gameOver != null) gameOver.SetActive(false);
    }

    public void ShowGameOver()
    {
        if(gameOver != null)
        {
            gameOver.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void UnPause ()
    {
        Time.timeScale = 1f;
    }
}
