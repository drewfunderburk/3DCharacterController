using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform player;

    [SerializeField] private float m_verticalSensitivity = 1;
    [SerializeField] private float m_horizontalSensitivity = 1;

    private float m_xRotation = 0;

    void Start()
    {
        // Lock cursor to game view
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get x and y mouse input
        float mouseX = Input.GetAxis("Mouse X") * m_horizontalSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * m_verticalSensitivity * Time.deltaTime;

        // Get new x rotation
        m_xRotation -= mouseY;

        // Clamp x rotation
        m_xRotation = Mathf.Clamp(m_xRotation, -90, 90);

        // Rotate camera on x
        transform.localRotation = Quaternion.Euler(m_xRotation, 0, 0);

        // Rotate player on z
        player.Rotate(Vector3.up * mouseX);
    }
}
