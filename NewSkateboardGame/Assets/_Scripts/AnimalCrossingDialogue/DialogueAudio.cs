using UnityEngine;
using TMPro;

public class DialogueAudio : MonoBehaviour
{
    public AudioClip[] voices;
    public AudioClip[] punctuations;
    [Space]
    public AudioSource voiceSource;
    public AudioSource punctuationSource;

    // Start is called before the first frame update
    void Start()
    {
        TMP_Animated.instance.onTextReveal.AddListener((newChar) => ReproduceSound(newChar));
    }

    public void ReproduceSound(char c)
    {
        if (char.IsPunctuation(c) && !punctuationSource.isPlaying)
        {
            voiceSource.Stop();
            punctuationSource.clip = punctuations[Random.Range(0, punctuations.Length)];
            punctuationSource.Play();
        }

        if (char.IsLetter(c) && !voiceSource.isPlaying)
        {
            punctuationSource.Stop();
            voiceSource.clip = voices[Random.Range(0, voices.Length)];
            voiceSource.Play();
        }

    }



}
