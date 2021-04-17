using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FirstPersonMouseLook : MonoBehaviour
{
    [Tooltip("Reference the player here. This will allow the script to rotate the player left and right.")]
    [SerializeField] private Transform _player;
    [SerializeField] private bool _captureCursorInGameView = true;
    [Space]
    [SerializeField] private float _verticalSensitivity = 1;
    [SerializeField] private float _horizontalSensitivity = 1;
    [Space]
    [SerializeField] private float _maximumVerticalAngle = 90;
    [SerializeField] private float _minimumVerticalAngle = -90;

    private float _xRotation = 0;

    void Start()
    {
        // Lock cursor to game view
        if (_captureCursorInGameView)
            Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get x and y mouse input
        float mouseX = Input.GetAxis("Mouse X") * _horizontalSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _verticalSensitivity * Time.deltaTime;

        // Get new x rotation based on Mouse Y input
        _xRotation -= mouseY;

        // Clamp x rotation
        _xRotation = Mathf.Clamp(_xRotation, _minimumVerticalAngle, _maximumVerticalAngle);

        // Rotate camera on x
        transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);

        // Rotate player on z
        _player.Rotate(Vector3.up * mouseX);
    }
}

[CustomEditor(typeof(FirstPersonMouseLook))]
class FirstPersonMouseLookEditor : Editor
{
    bool showText = true;

    public override void OnInspectorGUI()
    {
        // Display help text
        string helpText = "Place this script on the main camera, then child the camera to the player in a first person position.";
        string warnText = "This script will capture the cursor into the game view upon entering play mode and clicking the window. To get it back, press ESC.";

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