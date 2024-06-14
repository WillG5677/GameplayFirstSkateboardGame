using System;
using System.Timers;
using UnityEngine;

public class TimerWrapper : MonoBehaviour
{
    [SerializeField]
    private float durationSeconds = 1f;

    private Timer timerObj;
#nullable enable
    private DateTime? timerStartTime;
    private double pausedTimeRemaining;

    public event EventHandler? TimerElapsed;
#nullable disable
    private bool timerEndedEventNeedsFiring;

    public bool IsRunning => timerStartTime != null && !IsPaused;
    public bool IsPaused => pausedTimeRemaining > 0;

    public double TimeRemaining
    {
        get
        {
            if (pausedTimeRemaining > 0)
            {
                return pausedTimeRemaining;
            }
            else if (timerStartTime == null)
            {
                return 0;
            }
            else
            {
                TimeSpan elapsedTime = DateTime.Now - timerStartTime.Value;
                return timerObj.Interval / 1000.0 - elapsedTime.TotalSeconds;
            }
        }
    }

    public void StartTimer()
    {
        timerObj.Interval = durationSeconds * 1000;
        timerObj.Start();

        timerStartTime = DateTime.Now;
        pausedTimeRemaining = 0;
    }

    public void StopTimer()
    {
        timerObj.Stop();

        timerStartTime = null;
        pausedTimeRemaining = 0;
    }

    public void PauseTimer()
    {
        if (!IsRunning)
        {
            Debug.LogWarning($"{nameof(TimerWrapper)} on {gameObject} called pause when timer was not running");
            return;
        }

        timerObj.Stop();

        TimeSpan elapsedTime = DateTime.Now - timerStartTime.Value;
        pausedTimeRemaining = timerObj.Interval / 1000.0 - elapsedTime.TotalSeconds;
        if (pausedTimeRemaining < 0)
        {
            Debug.LogError($"On pause timer elapsed seconds ({elapsedTime.TotalSeconds}) was longer than duration ({timerObj.Interval / 1000.0}) ({pausedTimeRemaining})");
            pausedTimeRemaining = 0;
        }
    }

    public void ResumeTimer()
    {
        if (!IsPaused)
        {
            Debug.LogWarning($"{nameof(TimerWrapper)} on {gameObject} called resume when timer was not paused");
            return;
        }

        timerObj.Interval = pausedTimeRemaining * 1000;
        timerObj.Start();

        timerStartTime = DateTime.Now;
        pausedTimeRemaining = 0;
    }

    private void Awake()
    {
        timerObj = new Timer();
        timerObj.AutoReset = false;
        timerObj.Elapsed += OnTimerElapsed;
    }

    private void Update()
    {
        if (timerEndedEventNeedsFiring)
        {
            timerEndedEventNeedsFiring = false;

            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
#nullable enable
            EventHandler? timerElapsedEvent = TimerElapsed;
#nullable disable

            // Event will be null if there are no subscribers
            if (timerElapsedEvent != null)
            {
                timerElapsedEvent(this, EventArgs.Empty);
            }
        }
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        timerStartTime = null;
        pausedTimeRemaining = 0;
        timerEndedEventNeedsFiring = true;
    }
}
