using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI survivalTimeText;
    public TextMeshProUGUI bestTimeText;
    public GameObject gameOverPanel;

    private const string BestTimeKey = "BestTime";

    public void ShowGameOver(float elapsedTime)
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        float bestTime = PlayerPrefs.GetFloat(BestTimeKey, 0f);
        if (elapsedTime > bestTime)
        {
            bestTime = elapsedTime;
            PlayerPrefs.SetFloat(BestTimeKey, bestTime);
            PlayerPrefs.Save();
        }

        survivalTimeText.text = "Time: " + FormatTime(elapsedTime);
        bestTimeText.text = "Best: " + FormatTime(bestTime);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // change to your menu scene name
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}