using System;
using TMPro;
using UnityEngine;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timerText;
    [SerializeField]
    private TimerWrapper gameplayTimer;

    [SerializeField]
    private GameObject pauseCanvas;
    [SerializeField]
    private GameObject gameOverCanvas;

    private void Start()
    {
        GameStateManager.Instance.GameSessionPaused += OnGamePaused;
        GameStateManager.Instance.GameSessionResumed += OnGameResumed;
        GameStateManager.Instance.GameSessionTimeElapsed += OnGameOver;
        GameStateManager.Instance.GameSessionRestarted += OnGameRestarted;
    }

    private void Update()
    {
        double timeRemaining = gameplayTimer.TimeRemaining;
        TimeSpan timespan = TimeSpan.FromSeconds(timeRemaining);
        timerText.text = timespan.ToString("mm':'ss'.'ff");
    }

    private void OnGamePaused(object sender, EventArgs e)
    {
        pauseCanvas.SetActive(true);
    }

    private void OnGameResumed(object sender, EventArgs e)
    {
        pauseCanvas.SetActive(false);
    }

    private void OnGameOver(object sender, EventArgs e)
    {
        gameOverCanvas.SetActive(true);
    }

    private void OnGameRestarted(object sender, EventArgs e)
    {
        gameOverCanvas.SetActive(false);
    }
}
