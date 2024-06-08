using UnityEngine;

public class LookAtOrigin : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Vector3.zero);
    }
}
