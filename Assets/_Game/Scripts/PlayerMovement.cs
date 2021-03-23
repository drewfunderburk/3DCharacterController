using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // How fast we should allow the player to move under normal circumstances.
    // This is NOT used for calculations, only as a reference.
    [SerializeField] private float m_targetSpeed = 10;

    // Height in Unity Units to jump
    [SerializeField] private float m_jumpHeight = 2;

    // How much Physics.gravity should affect the player
    [SerializeField] private float m_gravityScale = 1;
    [Space]

    // Location of the ground check sphere relative to the player
    [SerializeField] private Vector3 m_groundCheck = new Vector3();

    // Radius of the ground check sphere
    [SerializeField] private float m_groundCheckRadius = 1;

    // Which layers should be considered ground
    [SerializeField] private LayerMask m_whatIsGround = new LayerMask();

    // Reference to the CharacterController
    private CharacterController m_controller;

    // The player's current speed. Modify and use THIS one
    private float m_speed;

    // Vector to store velocity 
    private Vector3 m_velocity;

    // Whether the player is grounded or not
    private bool m_isGrounded;

    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        m_speed = m_targetSpeed;
    }

    void Update()
    {
        m_isGrounded = Physics.CheckSphere(transform.position + m_groundCheck, m_groundCheckRadius, m_whatIsGround);

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
            m_velocity.y = Mathf.Sqrt(m_jumpHeight * -2f * (Physics.gravity.y * m_gravityScale));

        // Gravity
        m_velocity.y += (Physics.gravity.y * m_gravityScale) * Time.deltaTime;
        m_controller.Move(m_velocity * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + m_groundCheck, m_groundCheckRadius);
    }
}
