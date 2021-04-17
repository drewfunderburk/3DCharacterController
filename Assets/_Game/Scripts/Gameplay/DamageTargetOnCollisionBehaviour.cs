using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DamageTargetOnCollisionBehaviour : MonoBehaviour
{
    [Tooltip("Should this object log a warning to the console if it collides with something that doesn't have a HealthBehaviour?")]
    [SerializeField] private bool _showWarningsInConsole = false;

    [Tooltip("How much damage to deal to the collided object's HealthBehaviour")]
    [SerializeField] private float _damage = 1;

    [Tooltip("What layers this object should check when looking to deal damage")]
    [SerializeField] private LayerMask _whatToDamage = ~0;

    private void OnCollisionEnter(Collision collision)
    {
        // If the other GameObject's layer is in _whatToDamage
        if (((1 << collision.gameObject.layer) & _whatToDamage) != 0)
        {
            // Get a reference to the other object's HealthBehaviour
            HealthBehaviour otherHealth = collision.gameObject.GetComponent<HealthBehaviour>();

            // If it has a HealthBehaviour, damage it
            if (otherHealth)
                otherHealth.TakeDamage(_damage);
            // If not, log a warning
            else if (_showWarningsInConsole)
                Debug.LogWarning("Collided object " + collision.gameObject + " does not have a HealthBehaviour. Cannot damage it.");
        }
    }
}

[CustomEditor(typeof(DamageTargetOnCollisionBehaviour))]
public class DamageTargetOnCollisionBehaviourEditor : Editor
{
    private bool showText = true;

    public override void OnInspectorGUI()
    {
        // Declare help text
        string helpText = "Place this script on any object that you want to be able to cause damage to another via a collision.";
        string warnText = "In order to damage another object, it must have a HealthBehaviour script attached to it.";

        // Display help text
        showText = EditorGUILayout.BeginFoldoutHeaderGroup(showText, "Info");
        if (showText)
        {
            EditorGUILayout.HelpBox(helpText, MessageType.Info);
            EditorGUILayout.HelpBox(warnText, MessageType.Warning);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        // Display base inspector gui
        base.OnInspectorGUI();
    }
}