using UnityEngine;
using TMPro;

public class SurvivalTimer : MonoBehaviour
{
    public static SurvivalTimer Instance;
    
    public TextMeshProUGUI timerText;
    public GameOverUI gameOverUI;
    public float elapsedTime;
    private bool running = true;

    private void Awake()
    {
        Instance = this;
    }


    void Update()
    {
        if (RewindManager.Instance.IsBeingRewinded) return;
        if (!running) return;


        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        running = false;
        gameOverUI.ShowGameOver(elapsedTime);
    }
}