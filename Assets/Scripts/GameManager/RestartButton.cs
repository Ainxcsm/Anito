using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public GameOver gameOver;

    public void RestartGame()
    {
        if (gameOver != null)
        {
            gameOver.UnPause();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
