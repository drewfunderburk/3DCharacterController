using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class HealthBehaviour : MonoBehaviour
{
    [Tooltip("This object's maximum hp")]
    [SerializeField] private float _maxHealth;

    [Space]
    [Tooltip("Whether or not Invincibility Frames are turned on. This will make the object unable to take damage for a time after it has been damaged.")]
    [SerializeField] private bool _enableIFrames = true;

    [Tooltip("How long this object should be in iFrames after being damaged. During this time, it will be unable to take damage from any source.")]
    [SerializeField] private float _IFrameDuration = 1;

    public UnityEvent OnDamageTaken;

    private float _health;
    private bool _inIFrames = false;
    private float _IFrameTimer = 0;

    public float MaxHealth { get { return _maxHealth; } }
    public float Health { get { return _health; } }

    private void Start()
    {
        _health = _maxHealth;
    }

    private void Update()
    {
        // If this object is in iFrames
        if (_inIFrames)
        {
            // Increment timer
            _IFrameTimer += Time.deltaTime;

            // If we've been in iFrames longer than the duration
            if (_IFrameTimer > _IFrameDuration)
            {
                // Reset the timer and stop iFrames
                _IFrameTimer = 0;
                _inIFrames = false;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        // Do not take damage if in iFrames
        if (_inIFrames)
            return;

        // Enter iFrames if they are enabled
        if (_enableIFrames)
            _inIFrames = true;

        // Invoke OnTakeDamage
        OnDamageTaken.Invoke();

        // Take Damage
        _health -= damage;
        if (_health < 0)
            _health = 0;
    }

    public void Heal(float hp)
    {
        // Add hp to health
        _health += hp;

        // If health exceeds maxHealth, clamp it
        if (_health > _maxHealth)
            _health = _maxHealth;
    }
}

[CustomEditor(typeof(HealthBehaviour))]
public class HealthBehaviourEditor : Editor
{
    private bool showText = true;

    public override void OnInspectorGUI()
    {
        // Declare help text
        string helpText = @"This behaviour adds health to anything it is applied to. iFrames can be enabled to prevent rapidly damaging the same target over and over again.

Call this script's TakeDamage function to apply damage to this object.";

        // Display help text
        showText = EditorGUILayout.BeginFoldoutHeaderGroup(showText, "Info");
        if (showText)
            EditorGUILayout.HelpBox(helpText, MessageType.Info);

        EditorGUILayout.EndFoldoutHeaderGroup();

        // Display base inspector gui
        base.OnInspectorGUI();
    }
}