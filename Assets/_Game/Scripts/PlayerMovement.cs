using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_targetSpeed = 10;
    [SerializeField] private float m_jumpHeight = 2;
    [SerializeField] private float m_gravity = -9.81f;
    [Space]
    [SerializeField] private Vector3 m_groundCheck = new Vector3();
    [SerializeField] private float m_groundCheckRadius = 1;
    [SerializeField] private LayerMask m_whatIsGround = new LayerMask();

    private CharacterController m_controller;

    private float m_speed;
    private Vector3 m_velocity;

    private bool m_isGrounded;

    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        m_speed = m_targetSpeed;
    }

    void Update()
    {
        m_isGrounded = Physics.CheckSphere(m_groundCheck, m_groundCheckRadius, m_whatIsGround);

        // Reset y velocity when grounded
        if (m_isGrounded && m_velocity.y < 0)
            m_velocity.y = -2f;

        // Get x and y keyboard input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Move player on local axes
        Vector3 movement = transform.right * x + transform.forward * z;
        m_controller.Move(movement * m_speed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && m_isGrounded)
            m_velocity.y = Mathf.Sqrt(m_jumpHeight * -2f * m_gravity);

        // Gravity
        m_velocity.y += m_gravity * Time.deltaTime;
        m_controller.Move(m_velocity * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + m_groundCheck, m_groundCheckRadius);
    }
}
