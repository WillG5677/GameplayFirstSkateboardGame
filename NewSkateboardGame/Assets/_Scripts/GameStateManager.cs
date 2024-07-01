using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

#nullable enable
    public EventHandler? GameSessionPaused;
    public EventHandler? GameSessionResumed;
    public EventHandler? GameSessionTimeElapsed;
    public EventHandler? GameSessionRestarted;
#nullable disable

    [SerializeField]
    private float gameStartOnLoadDelaySeconds = 0.66f;

    [SerializeField]
    private TimerWrapper gameplayTimer;

    private bool gamePaused;
    [SerializeField] private string winSceneName = string.Empty;

    public void RestartGameSession()
    {
        Time.timeScale = 1;
        gameplayTimer.StartTimer();

        // Make a temporary copy of the event to avoid possibility of
        // a race condition if the last subscriber unsubscribes
        // immediately after the null check and before the event is raised.
#nullable enable
        EventHandler? gameRestartedEvent = GameSessionRestarted;
#nullable disable

        // Event will be null if there are no subscribers
        if (gameRestartedEvent != null)
        {
            gameRestartedEvent(this, EventArgs.Empty);
        }
    }

    public void PauseGameSession()
    {
        gamePaused = true;
        Time.timeScale = 0;
        gameplayTimer.PauseTimer();

        // Make a temporary copy of the event to avoid possibility of
        // a race condition if the last subscriber unsubscribes
        // immediately after the null check and before the event is raised.
#nullable enable
        EventHandler? gamePausedEvent = GameSessionPaused;
#nullable disable

        // Event will be null if there are no subscribers
        if (gamePausedEvent != null)
        {
            gamePausedEvent(this, EventArgs.Empty);
        }
    }

    public void ResumeGameSession()
    {
        gamePaused = false;
        Time.timeScale = 1;
        gameplayTimer.ResumeTimer();

        // Make a temporary copy of the event to avoid possibility of
        // a race condition if the last subscriber unsubscribes
        // immediately after the null check and before the event is raised.
#nullable enable
        EventHandler? gameResumedEvent = GameSessionResumed;
#nullable disable

        // Event will be null if there are no subscribers
        if (gameResumedEvent != null)
        {
            gameResumedEvent(this, EventArgs.Empty);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        gameplayTimer.TimerElapsed += OnGameplayTimerElapsed;
    }

    private void Start()
    {
        StartCoroutine(DelayedGameStart());
    }

    private void Update()
    {
        if (!gamePaused && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGameSession();
        }

        if (ChaosManager.Instance.chaos == ChaosManager.Instance.MAX_CHAOS) {
            SceneManager.LoadScene(winSceneName);
        }
    }

    private IEnumerator DelayedGameStart()
    {
        yield return new WaitForSeconds(gameStartOnLoadDelaySeconds);
        RestartGameSession();
    }

    private void OnGameplayTimerElapsed(object sender, EventArgs e)
    {
        Time.timeScale = 0;

        // Make a temporary copy of the event to avoid possibility of
        // a race condition if the last subscriber unsubscribes
        // immediately after the null check and before the event is raised.
#nullable enable
        EventHandler? gameElapsedEvent = GameSessionTimeElapsed;
#nullable disable

        // Event will be null if there are no subscribers
        if (gameElapsedEvent != null)
        {
            gameElapsedEvent(this, EventArgs.Empty);
        }
    }
}
