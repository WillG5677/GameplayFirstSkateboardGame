using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosManager : MonoBehaviour 
{
    public static ChaosManager Instance { get; private set; }
    public float chaos = 0f;

    public float MAX_CHAOS;


    [Header("Sound")]
    [SerializeField] private AudioClip music1;
    [SerializeField] private AudioClip music2;
    [SerializeField] private AudioClip music3;
    private AudioSource audioSource;

    // creating a new chaos for when the game is played
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: if you want the ChaosManager to persist between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IncreaseChaos(float amount)
    {
        chaos += amount;
        Debug.Log("Chaos increased! Current chaos level: " + chaos);
    }

    private void CheckChaosLevels()
    {
        if (chaos >= 10f && chaos < 20f)
        {
            SwitchMusic();
        }
        // Add more conditions for different chaos levels
        else if (chaos >= 20f)
        {
            PerformAnotherAction();
        }
        // Continue adding more conditions for different actions
    }

    private void SwitchMusic()
    {
        if (audioSource.clip != music2)
        {
            audioSource.clip = music2;
            audioSource.Play();
            Debug.Log("Switched to chaos music");
        }
    }

    private void PerformAnotherAction()
    {
        // Implement your additional actions here
        Debug.Log("Performing another action due to high chaos level");
    }
}
