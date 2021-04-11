using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementRidigbody : MonoBehaviour
{
    [Tooltip("Player's base speed")]
    [SerializeField] private float _baseSpeed;

    [Tooltip("How fast the player approaches top speed")]
    [SerializeField] private float _acceleration;

/*
    [Tooltip("How many seconds it should take for the player to come to a complete stop")]
    [SerializeField] private float _decelleration;
*/

    [Tooltip("Hight the player's jump should reach at its apex")]
    [SerializeField] private float _jumpHeight;

    [Tooltip("Amount of movement velocity to apply while airborn")]
    [Range(0, 1)]
    [SerializeField] private float _airMovementScale;

    [Tooltip("The position around which the ground check will be performed")]
    [SerializeField] private Vector3 _groundCheckOffset;

    [Tooltip("The radius of the ground check")]
    [SerializeField] private float _groundCheckRadius;

    [Tooltip("What is considered ground")]
    [SerializeField] private LayerMask _whatIsGround;

    private Rigidbody _rigidbody;

    // The actual speed used in calculations.
    // _baseSpeed is to provide a baseline to return to in case of sprinting or other movespeed changes
    private float _targetSpeed;

    private bool _isGrounded;
    private bool _canJump;

    private float _jumpTimer;
    private float _jumpDelay;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _targetSpeed = _baseSpeed
;
        _isGrounded = false;
        _canJump = true;
        _jumpTimer = 0;
        _jumpDelay = 0.5f;
    }

    private void Update()
    {
        // Increment timers
        _jumpTimer += Time.deltaTime;

        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal") * _targetSpeed;
        float vertical = Input.GetAxisRaw("Vertical") * _targetSpeed;
        bool jump = Input.GetButtonDown("Jump");

        // Check if player is grounded
        _isGrounded = Physics.CheckSphere(transform.position + _groundCheckOffset, _groundCheckRadius, _whatIsGround);
        
        // Apply speed reduction if the player is grounded
        if (_isGrounded)
        {
            Vector3 newVelocity = _rigidbody.velocity;
            if (horizontal == 0)
                newVelocity.x = 0;
            if (vertical == 0)
                newVelocity.z = 0;
            
            _rigidbody.velocity = newVelocity;
        }


        // Allow jumping if the player is grounded and jump delay has been exceeded
        _canJump =  (_jumpTimer > _jumpDelay) && _isGrounded ? true : false;

        // If jump is pressed and the player can jump, do so
        if (jump && _canJump)
        {
            // Reset jump timer
            _jumpTimer = 0;

            // Calculate jump velocity to achieve desired height
            float jumpVelocity = Mathf.Sqrt(_jumpHeight * -2f * Physics.gravity.y);

            // Set the rigidbody's velocity
            Vector3 newVelocity = new Vector3(_rigidbody.velocity.x, jumpVelocity, _rigidbody.velocity.z);
            _rigidbody.velocity = newVelocity;
        }

        // Create a new Vector3 to store forward and sideways movement
        Vector3 movement = new Vector3(horizontal * _targetSpeed, _rigidbody.velocity.y, vertical * _targetSpeed);

        // Apply air movement scale if player is airborn
        if (!_isGrounded)
            movement *= _airMovementScale;
        
        // Add force to rigidbody
        _rigidbody.AddForce(movement * _acceleration * Time.deltaTime, ForceMode.Acceleration);

        // Store the x and z movement of the rigidbody
        Vector2 velocity2D = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z);

        // If the x and z movement have exceeded our target speed, clamp
        if (velocity2D.magnitude > _targetSpeed)
        {
            velocity2D = velocity2D.normalized * _targetSpeed;
            _rigidbody.velocity = new Vector3(velocity2D.x, _rigidbody.velocity.y, velocity2D.y);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw ground check gizmo
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + _groundCheckOffset, _groundCheckRadius);
    }
}
