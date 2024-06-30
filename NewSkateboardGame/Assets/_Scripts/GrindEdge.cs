using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class GrindEdge : MonoBehaviour
{
    public Vector3 attachPosition;
    public Vector3 approachVector = new Vector3(1, 0, 0);
    public float approachAngleRange = 60f;
    public bool reverseSpline;

    public SplineContainer GrindSpline => grindSpline;

    private SplineContainer grindSpline;

    private void Reset()
    {
        attachPosition = transform.position;
    }

    private void Awake()
    {
        grindSpline = GetComponentInParent<SplineContainer>();
        if (grindSpline == null)
        {
            Debug.LogError($"{nameof(GrindEdge)} on {gameObject} is not a child of a spline - destroying self");
            Destroy(gameObject);
        }
    }
}
