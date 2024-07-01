using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NarrativeIntroController : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private List<PanelTimingInfo> panelTimings = new List<PanelTimingInfo>();

    [SerializeField]
    private string gameplaySceneName = string.Empty;

    private int currentPanel = -1;
    private Coroutine coroutine;

    private void OnValidate()
    {
        if (panelTimings.Count < canvas.transform.childCount)
        {
            List<GameObject> panels = new List<GameObject>();
            for (int c = 0; c < canvas.transform.childCount; c++)
            {
                GameObject panel = canvas.transform.GetChild(c).gameObject;
                if (panel.name.IndexOf("Panel", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    panels.Add(panel);
                }
            }

            for (int p = panelTimings.Count; p < panels.Count; p++)
            {
                panelTimings.Add(new PanelTimingInfo() { panel = panels[p] });
            }
        }
    }

    private void Update()
    {
        if (currentPanel < 0 || currentPanel >= panelTimings.Count
            || Time.timeScale == 0f)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            SkipPanel();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }    
    }

    public void PlayNarrative()
    {
        canvas.gameObject.SetActive(true);
        for (int c = 0; c < canvas.transform.childCount; c++)
        {
            canvas.transform.GetChild(c).gameObject.SetActive(false);
        }
        TransitionPanel();
    }

    public void SkipNarrative()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        TogglePause();
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void TogglePause()
    {
        bool alreadyPaused = Time.timeScale == 0f;
        Time.timeScale = alreadyPaused ? 1f : 0f;
        pauseMenu.SetActive(!alreadyPaused);
    }

    private void TransitionPanel()
    {
        if(currentPanel >= 0 && currentPanel < panelTimings.Count - 1)
        {
            panelTimings[currentPanel].panel.SetActive(false);
        }
        currentPanel++;
        if (currentPanel < panelTimings.Count)
        {
            panelTimings[currentPanel].panel.SetActive(true);
            coroutine = StartCoroutine(TransitionPanelCoroutine());
        }
        else
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
    }

    private void SkipPanel()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        TransitionPanel();
    }

    private IEnumerator TransitionPanelCoroutine()
    {
        yield return new WaitForSeconds(panelTimings[currentPanel].displayDuration);
        TransitionPanel();
    }

    [System.Serializable]
    private class PanelTimingInfo
    {
        public GameObject panel;
        public float displayDuration = 10f;
    }
}
