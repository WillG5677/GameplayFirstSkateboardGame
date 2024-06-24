using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySpeedUI : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    Text speedText;

    // Start is called before the first frame update
    void Start()
    {
        speedText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        float speed = rb.velocity.magnitude;
        speedText.text = "Speed: " + speed.ToString("F1");
    }
}
