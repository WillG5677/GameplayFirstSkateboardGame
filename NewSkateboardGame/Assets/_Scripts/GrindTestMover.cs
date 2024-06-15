using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineAnimate))]
public class GrindTestMover : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 2.5f;
    [SerializeField]
    private float turnSpeed = 1f;

    private SplineAnimate splineAnimator;
    private readonly List<KeyValuePair<GrindEdge, Collider>> nearbyGrindEdges = new List<KeyValuePair<GrindEdge, Collider>>();
    private bool isGrinding = false;

    private GrindEdge bestGrindEdge;

    [SerializeField]
    private TextMeshProUGUI label;

    private void Awake()
    {
        splineAnimator = GetComponent<SplineAnimate>();
    }

    private void Update()
    {
        bestGrindEdge = GetBestGrindEdge();
        if (bestGrindEdge != null && Input.GetKeyDown(KeyCode.Space))
        {
            isGrinding = true;
            splineAnimator.Container = bestGrindEdge.GrindSpline;
            splineAnimator.Play();
        }

        if (!isGrinding)
        {
            transform.Rotate(transform.up, Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime);
            transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        if (bestGrindEdge == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(bestGrindEdge.attachPosition, bestGrindEdge.attachPosition + bestGrindEdge.approachVector);
        Gizmos.color = Color.green;
        //Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        Gizmos.DrawLine(transform.position, transform.position + (bestGrindEdge.attachPosition - transform.position).normalized);
    }

    private void OnTriggerEnter(Collider other)
    {
        GrindEdge grindEdge = other.GetComponent<GrindEdge>();
        if (grindEdge == null)
        {
            return;
        }

        nearbyGrindEdges.Add(new KeyValuePair<GrindEdge, Collider>(grindEdge, other));
    }

    private void OnTriggerExit(Collider other)
    {
        GrindEdge grindEdge = other.GetComponent<GrindEdge>();
        if (grindEdge == null)
        {
            return;
        }

        nearbyGrindEdges.Remove(new KeyValuePair<GrindEdge, Collider>(grindEdge, other));
    }

    private GrindEdge GetBestGrindEdge()
    {
        GrindEdge bestGrindEdge = null;
        if (nearbyGrindEdges.Count == 0)
        {
            return bestGrindEdge;
        }

        float smallestAngle = float.MaxValue;
        foreach (KeyValuePair<GrindEdge, Collider> nearbyGrindEdge in nearbyGrindEdges)
        {
            float rightAlignAngle = Vector3.Angle(transform.forward,
                Quaternion.Euler(0f, nearbyGrindEdge.Key.approachAngleRange / 2f, 0f) * nearbyGrindEdge.Key.approachVector);
            Vector3 cross = Vector3.Cross(transform.forward,
                Quaternion.Euler(0f, nearbyGrindEdge.Key.approachAngleRange / 2f, 0f) * nearbyGrindEdge.Key.approachVector);
            if (cross.y < 0f)
            {
                rightAlignAngle *= -1f;
            }

            float leftAlignAngle = Vector3.Angle(transform.forward,
                Quaternion.Euler(0f, -nearbyGrindEdge.Key.approachAngleRange / 2f, 0f) * nearbyGrindEdge.Key.approachVector);
            cross = Vector3.Cross(transform.forward,
                Quaternion.Euler(0f, -nearbyGrindEdge.Key.approachAngleRange / 2f, 0f) * nearbyGrindEdge.Key.approachVector);
            if (cross.y < 0f)
            {
                leftAlignAngle *= -1f;
            }

            float faceAngle = Vector3.Angle(transform.forward, (nearbyGrindEdge.Key.attachPosition - transform.position).normalized);
            bool angleGood = faceAngle >= Mathf.Min(leftAlignAngle, rightAlignAngle) &&
                faceAngle <= Mathf.Max(leftAlignAngle, rightAlignAngle) &&
                faceAngle <= nearbyGrindEdge.Key.approachAngleRange / 2f;

            if (!angleGood)
            {
                continue;
            }

            if (faceAngle < smallestAngle)
            {
                smallestAngle = faceAngle;
                bestGrindEdge = nearbyGrindEdge.Key;
            }
        }
        label.text = smallestAngle.ToString();
        return bestGrindEdge;
    }
}
