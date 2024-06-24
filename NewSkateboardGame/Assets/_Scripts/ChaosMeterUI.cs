using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChaosMeterUI : MonoBehaviour
{
    private Slider chaosMeter;
    [SerializeField] private Text chaosValueText;
    private ChaosManager chaosManager;

    // Start is called before the first frame update
    void Start()
    {
        chaosMeter = GetComponent<Slider>();
        chaosManager = ChaosManager.Instance;

        // Initialize the Slider
        chaosMeter.minValue = 0f;
        chaosMeter.maxValue = 100f;

        if (chaosValueText != null) {
            chaosValueText.text = "Chaos: " + chaosManager.chaos.ToString("F1");
        }
    }

    // Update is called once per frame
    void Update()
    {
        chaosMeter.value = chaosManager.chaos;

        if (chaosValueText != null) {
            chaosValueText.text = "Chaos: " + chaosManager.chaos.ToString("F1");
        }
    }
}
