using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GrindEdge))]
public class GrindEdgeEditor : Editor
{
    private GrindEdge editingGrindEdge;

    private void OnEnable()
    {
        editingGrindEdge = (GrindEdge)target;
    }

    private void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();
        Handles.color = Color.blue;
        Vector3 newAttachPoint = Handles.FreeMoveHandle(editingGrindEdge.attachPosition,
            HandleUtility.GetHandleSize(editingGrindEdge.transform.position) * 0.25f,
            Vector3.one * 0.5f,
            Handles.SphereHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(editingGrindEdge, "Moved grind attach position");
            editingGrindEdge.attachPosition = newAttachPoint;
        }

        float halfAngle = editingGrindEdge.approachAngleRange / 2f;
        Vector3 startRangeDirection = Quaternion.Euler(0f, -halfAngle, 0f) * editingGrindEdge.approachVector;
        Handles.DrawWireArc(editingGrindEdge.attachPosition,
            Vector3.up,
            -startRangeDirection,
            editingGrindEdge.approachAngleRange,
            0.3f);

        EditorGUI.BeginChangeCheck();
        Quaternion newRotation = Handles.RotationHandle(Quaternion.identity,
            editingGrindEdge.attachPosition);
        if (EditorGUI.EndChangeCheck())
        {
            Vector3 newApproachVector = newRotation * editingGrindEdge.approachVector;
            newApproachVector.y = 0f;
            newApproachVector.Normalize();
            Undo.RecordObject(editingGrindEdge, "Rotated grind approach vector");
            editingGrindEdge.approachVector = newApproachVector;
        }

        Handles.color = Color.white;
        float arrowSize = HandleUtility.GetHandleSize(editingGrindEdge.transform.position);
        Vector3 arrowPosition = editingGrindEdge.attachPosition + (-arrowSize * editingGrindEdge.approachVector.normalized);
        Handles.ArrowHandleCap(0,
            arrowPosition,
            Quaternion.LookRotation(editingGrindEdge.approachVector),
            arrowSize,
            EventType.Repaint);
    }
}
