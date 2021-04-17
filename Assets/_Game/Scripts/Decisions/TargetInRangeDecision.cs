using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class TargetInRangeDecision : MonoBehaviour
{
    [Tooltip("The object whos range we care about")]
    [SerializeField] private Transform _target;

    [Tooltip("What distance is considered \"in range\"")]
    [SerializeField] private float _range = 10f;
    [Space]
    [SerializeField] private bool _showRangeGizmo = true;
    [Space]

    // Functions to call when target passes within range
    public UnityEvent OnInRange;

    // Functions to call when target passes out of range
    public UnityEvent OnOutOfRange;

    private bool _inRange = false;

    /// <summary>
    /// Returns whether or not the script has a target
    /// </summary>
    public bool HasTarget() { return _target; }

    private void Update()
    {
        // If there is no target, do nothing
        if (_target) return;

        // If the target passes within range and was not already in range, invoke OnInRange
        if (Vector3.Distance(transform.position, _target.position) < _range && _inRange == false)
        {
            _inRange = true;
            OnInRange.Invoke();
        }
        // If the target passes out of range and was not already out of range, invoke OnOutOfRange
        else if (Vector3.Distance(transform.position, _target.position) > _range && _inRange == true)
        {
            _inRange = false;
            OnOutOfRange.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw range gizmo
        if (_showRangeGizmo)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _range);
        }
    }
}

[CustomEditor(typeof(TargetInRangeDecision))]
public class TargetInRangeDecisionEditor : Editor
{
    private bool showText = true;

    public override void OnInspectorGUI()
    {
        // Get reference to the script
        TargetInRangeDecision script = target as TargetInRangeDecision;

        // Declare help text
        string helpText = @"This decision will fire off a pair of events based on the range to a target.

When the target passes within the range specified, this script will invoke the OnInRange event once. When it passes out of range, OnOutOfRange will be invoked once.";

        // Display help text
        showText = EditorGUILayout.BeginFoldoutHeaderGroup(showText, "Info");
        if (showText)
            EditorGUILayout.HelpBox(helpText, MessageType.Info);

        EditorGUILayout.EndFoldoutHeaderGroup();

        // Show no target warning
        if (!script.HasTarget())
            EditorGUILayout.HelpBox("Script has no target!", MessageType.Warning);

        // Display base inspector gui
        base.OnInspectorGUI();
    }
}