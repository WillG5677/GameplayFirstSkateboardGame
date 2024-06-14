using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class GrindEdge : MonoBehaviour
{
    private Spline thisSpline;

    private void Awake()
    {
        thisSpline = GetComponent<SplineContainer>().Spline;
    }
}
