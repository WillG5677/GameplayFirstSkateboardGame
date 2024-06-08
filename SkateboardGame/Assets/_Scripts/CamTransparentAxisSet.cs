using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CamTransparentAxisSet : MonoBehaviour
{
    public Vector3 transparentAxis;

    private Camera thisCam;

    private void Awake()
    {
        thisCam = GetComponent<Camera>();
        thisCam.transparencySortMode = TransparencySortMode.CustomAxis;
    }

    private void Update()
    {
        thisCam.transparencySortAxis = transparentAxis;
    }
}
