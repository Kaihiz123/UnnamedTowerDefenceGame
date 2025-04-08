using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    public float gameTime = 0f;
    private bool isPaused = false;

    private void Start()
    {
        gameTime = 0f;
        UpdateTimerDisplay();
        
        // Request statistics reset at the start of a new round
        if (StatisticsTracker.Instance != null)
        {
            StatisticsTracker.Instance.RequestStatisticsReset();
        }
    }

    private void Update()
    {
        if (!isPaused)
        {
            gameTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    public void PauseTimer(bool paused)
    {
        isPaused = paused;
    }

    public void StopTimer()
    {
        isPaused = true;
    }

    public float GetGameTime()
    {
        return gameTime;
    }

    public string GetFormattedTime()
    {
        int hours = Mathf.FloorToInt(gameTime / 3600f);
        int minutes = Mathf.FloorToInt((gameTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);

        if (hours > 0)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
        else
        {
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = GetFormattedTime();
        }
    }
}