using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinStateController : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject finalPanelOverlay;
    [SerializeField]
    private TextMeshProUGUI flipsText;

    [SerializeField]
    private List<PanelTimingInfo> panelTimings = new List<PanelTimingInfo>();

    [SerializeField]
    private string mainMenuSceneName = string.Empty;

    private int currentPanel = -1;
    private Coroutine coroutine;

    private bool OnFinalPanel => currentPanel >= panelTimings.Count - 1;

    [SerializeField]
    private GameObject leftPrompt;
    [SerializeField]
    private GameObject rightPrompt;

    private bool lastKeyLeft;

    private ulong flips;

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

    private void Start()
    {
        PlayNarrative();
    }

    private void Update()
    {
        if (currentPanel < 0 || currentPanel >= panelTimings.Count
            || Time.timeScale == 0f)
        {
            return;
        }

        if (!OnFinalPanel)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SkipPanel();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
        else
        {
            if ((lastKeyLeft && Input.GetKeyDown(KeyCode.RightArrow))
                || (!lastKeyLeft && Input.GetKeyDown(KeyCode.LeftArrow)))
            {
                DoAFlip();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
    }

    public void PlayNarrative()
    {
        for (int c = 0; c < canvas.transform.childCount; c++)
        {
            canvas.transform.GetChild(c).gameObject.SetActive(false);
        }
        TransitionPanel();
    }

    public void ReturnToMainMenu()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        TogglePause();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void TogglePause()
    {
        bool alreadyPaused = Time.timeScale == 0f;
        Time.timeScale = alreadyPaused ? 1f : 0f;
        pauseMenu.SetActive(!alreadyPaused);
    }

    private void TransitionPanel()
    {
        if (currentPanel >= 0 && currentPanel < panelTimings.Count - 1)
        {
            panelTimings[currentPanel].panel.SetActive(false);
        }
        currentPanel++;
        if (currentPanel < panelTimings.Count)
        {
            panelTimings[currentPanel].panel.SetActive(true);
            if (currentPanel < panelTimings.Count - 1)
            {
                coroutine = StartCoroutine(TransitionPanelCoroutine());
            }
            else
            {
                finalPanelOverlay.SetActive(true);
                leftPrompt.SetActive(!lastKeyLeft);
                rightPrompt.SetActive(lastKeyLeft);
                flipsText.text = $"Rotation: {flips}";
            }
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

    private void DoAFlip()
    {
        lastKeyLeft = !lastKeyLeft;
        leftPrompt.SetActive(!lastKeyLeft);
        rightPrompt.SetActive(lastKeyLeft);
        Vector3 localScale = panelTimings[currentPanel].panel.transform.localScale;
        localScale.y *= -1f;
        panelTimings[currentPanel].panel.transform.localScale = localScale;
        flips += 180;
        flipsText.text = $"Rotation: {flips}";
    }

    [System.Serializable]
    private class PanelTimingInfo
    {
        public GameObject panel;
        public float displayDuration = 3f;
    }
}
