using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class CharacterController3D : MonoBehaviour
{
    #region Serialized Fields
    [Tooltip("The name of the input axis used for horizontal movement. Horizontal by default")]
    [SerializeField] private string _horizontalAxisName = "Horizontal";
    [Tooltip("The name of the input axis used for vertical movement. Vertical by default")]
    [SerializeField] private string _verticalAxisName = "Vertical";
    [Tooltip("The name of the input axis used for jumping. Jump by default")]
    [SerializeField] private string _jumpAxisName = "Jump";
    [Tooltip("The name of the input axis used for sprinting. Fire3 by default")]
    [SerializeField] private string _sprintAxisName = "Fire3";

    [Space]
    [Tooltip("The base speed this character should move at. Sprinting will scale from this value")]
    [SerializeField] private float _baseMoveSpeed = 5;

    [Tooltip("How quickly should we reach top speed")]
    [SerializeField] private float _acceleration = 2000;

    [Tooltip("What percentage of Base Move Speed sprinting should be. A value of 2 will result in sprinting being twice as fast as normal movement.")]
    [SerializeField] private float _sprintSpeedMultiplier = 2.0f;

    [Space]
    [Tooltip("Whether or not this character is allowed to jump")]
    [SerializeField] private bool _allowJumping = true;

    [Tooltip("How high in Units this character can jump")]
    [SerializeField] private float _jumpHeight = 2;

    [Min(0.1f)]
    [Tooltip("How many seconds must pass before the character is allowed to jump again. (Minimum of 0.1 to prevent physics bugs)")]
    [SerializeField] private float _jumpDelay = 0.1f;

    [Range(0, 1)]
    [Tooltip("How much movement should be allowed while airborn. 0 for none, 1 for full Base Move Speed")]
    [SerializeField] private float _airMovementScale = 0.3f;

    [Space]
    [Tooltip("Show the wire sphere indicating the ground check position and radius")]
    [SerializeField] private bool _showGroundCheckGizmo = true;

    [Tooltip("The position from which we should check if the character is grounded.")]
    [SerializeField] private Vector3 _groundCheckPosition = new Vector3(0, -0.7f, 0);

    [Tooltip("How large of an area we should check for ground.")]
    [SerializeField] private float _groundCheckRadius = 0.5f;

    [Tooltip("What layers are considered ground. If this is not set properly, the character will always think it is airborn.")]
    [SerializeField] private LayerMask _whatIsGround = ~0;
    #endregion

    private Rigidbody _rigidbody;
    private float _targetSpeed;     // This speed is used for calculations, _baseMoveSpeed is used as a reference to return this one to
    private bool _isGrounded;
    private bool _canJump;
    private float _jumpTimer;

    private void Start()
    {
        // Get this character's rigidbody
        _rigidbody = GetComponent<Rigidbody>();

        // Freeze the rigidbody's rotation on the X and Z axis, preventing it from falling over
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Initialize default values
        _targetSpeed = _baseMoveSpeed;
        _isGrounded = false;
        _canJump = true;
        _jumpTimer = 0;
    }

    private void Update()
    {
        // Get input
        float horizontal = Input.GetAxisRaw(_horizontalAxisName);
        float vertical = Input.GetAxisRaw(_verticalAxisName);
        bool jump = Input.GetButtonDown(_jumpAxisName);
        bool sprint = Input.GetButton(_sprintAxisName);

        PerformGroundCheck();
        IncrementTimers();
        ApplyGroundedSpeedReduction(horizontal, vertical);
        Jump(jump);
        Sprint(sprint);
        Movement(horizontal, vertical);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw ground check gizmo
        if (_showGroundCheckGizmo)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + _groundCheckPosition, _groundCheckRadius);
        }
    }

    /// <summary>
    /// Check if the player is grounded by checking a sphere at _groundCheckPosition to see if it collides with anything
    /// </summary>
    private void PerformGroundCheck()
    {
        // Check if the character is currently grounded
        _isGrounded = Physics.CheckSphere(transform.position + _groundCheckPosition, _groundCheckRadius, _whatIsGround);
    }

    /// <summary>
    /// Apply _targetSpeed increase if we are sprinting
    /// </summary>
    private void Sprint(bool sprint)
    {
        // No sprinting in the air
        if (!_isGrounded)
        {
            _targetSpeed = _baseMoveSpeed;
            return;
        }

        if (sprint)
            _targetSpeed = _baseMoveSpeed * _sprintSpeedMultiplier;
        else
            _targetSpeed = _baseMoveSpeed;
    }

    /// <summary>
    /// Set the character's velocity based on the given inputs
    /// </summary>
    private void Movement(float horizontal, float vertical)
    {
        // Create a new Vector3 to store forward and sideways movement
        Vector3 movement = new Vector3(horizontal * _acceleration, _rigidbody.velocity.y, vertical * _acceleration);

        // Apply air movement scale if player is airborn
        if (!_isGrounded)
            movement *= _airMovementScale;

        // Add force to rigidbody
        _rigidbody.AddForce(movement * Time.deltaTime, ForceMode.Acceleration);

        // Store the x and z movement of the rigidbody
        Vector2 velocity2D = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z);

        // If the x and z movement have exceeded our target speed, clamp
        if (velocity2D.magnitude > _targetSpeed)
        {
            velocity2D = velocity2D.normalized * _targetSpeed;
            _rigidbody.velocity = new Vector3(velocity2D.x, _rigidbody.velocity.y, velocity2D.y);
        }
    }

    /// <summary>
    /// Apply jump force
    /// </summary>
    private void Jump(bool jump)
    {
        // Allow jumping if the player is grounded and jump delay has been exceeded
        _canJump = (_jumpTimer > _jumpDelay) && _isGrounded ? true : false;

        // If jump is pressed and the player can jump, do so
        if (_allowJumping && jump && _canJump)
        {
            // Reset jump timer
            _jumpTimer = 0;

            // Calculate jump velocity to achieve desired height
            float jumpVelocity = Mathf.Sqrt(_jumpHeight * -2f * Physics.gravity.y);

            // Set the rigidbody's velocity directly rather than add force for a more snappy and precise jump
            Vector3 newVelocity = new Vector3(_rigidbody.velocity.x, jumpVelocity, _rigidbody.velocity.z);
            _rigidbody.velocity = newVelocity;
        }
    }

    /// <summary>
    /// If the player is grounded, reduce velocity to 0 on axes that have no input to prevent sliding
    /// </summary>
    private void ApplyGroundedSpeedReduction(float horizontal, float vertical)
    {
        if (_isGrounded)
        {
            Vector3 newVelocity = _rigidbody.velocity;
            // If there is no horizontal input, set the horizontal velocity to 0
            if (horizontal == 0)
                newVelocity.x = 0;
            // If there is no vertical input, set the vertical velocity to 0
            if (vertical == 0)
                newVelocity.z = 0;

            _rigidbody.velocity = newVelocity;
        }
    }

    /// <summary>
    /// Increment all timers by Time.deltaTime
    /// </summary>
    private void IncrementTimers()
    {
        _jumpTimer += Time.deltaTime;
    }
}

[CustomEditor(typeof(CharacterController3D))]
public class CharacterController3DEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Display help text
        string helpText = @"
This 3D Character Controller uses a Rigidbody and the physics system to move about. As such, it will require a Rigidbody component to function properly. 

Reasonable default values have been given for most field under default physics settings to give you an idea of where you should expect them to be.
";
        EditorGUILayout.HelpBox(helpText, MessageType.Info);

        // Display base inspector gui
        base.OnInspectorGUI();
    }
}