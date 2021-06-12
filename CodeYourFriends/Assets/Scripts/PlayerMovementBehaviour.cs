using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;

public class PlayerMovementBehaviour : MonoBehaviour
{
    [Header("Properties")] 
    [SerializeField]
    private float speedForce = 10.0f;
    [SerializeField]
    private float maxSpeed = 10.0f;
    [SerializeField] 
    private float jumpForce = 5.0f;
    [SerializeField]
    private float directionalJumpForce = 2.5f;
    [SerializeField] 
    private float fallMultiplier = 2.0f;
    [SerializeField]
    private float airborneMovementReduction = 3.0f;
    [SerializeField]
    private float walledGlidingSpeed = .5f;
    [SerializeField] 
    private float walledMaxGlidingSpeed = 2.0f;

    [Header("Collision")]
    
    [SerializeField] 
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask wallLayer;

    [Header("Input Actions")]
    [SerializeField]
    private InputActionMap walkMap;

    [SerializeField]
    private InputActionMap jumpMap;

    // non editor properties
    private Vector2 _dir;

    // components
    private Rigidbody _rb;
    private CapsuleCollider _collider;
    
    // children 
    private GetCollisionScript _groundCheck;
    private GetCollisionScript _leftWallCheck;
    private GetCollisionScript _rightWallCheck;

    protected void Awake()
    {
        // walkMap.Enable();
        // jumpMap.Enable();

        foreach (var action in walkMap)
        {
            action.performed += OnWalk;
            action.canceled += OnStop;
        }

        foreach (var action in jumpMap)
        {
            action.performed += OnJump;
        }

        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();

        _groundCheck = transform.Find("GroundCheck").GetComponent<GetCollisionScript>();
        _leftWallCheck = transform.Find("LeftWallCheck").GetComponent<GetCollisionScript>();
        _rightWallCheck = transform.Find("RightWallCheck").GetComponent<GetCollisionScript>();
    }

    protected void FixedUpdate()
    {
        var horSpeed = _groundCheck.IsColliding ? _dir.x * speedForce : _dir.x * speedForce / airborneMovementReduction;
        horSpeed = _leftWallCheck.IsColliding && _dir.x < 0 ? 0 : horSpeed;
        horSpeed = _rightWallCheck.IsColliding && _dir.x > 0 ? 0 : horSpeed;
        
        _rb.AddForce(new Vector3(0, 0, horSpeed));

        var vel = _rb.velocity;
        
        if (_leftWallCheck.IsColliding || _rightWallCheck.IsColliding)
        {
            vel.y = vel.y < -walledMaxGlidingSpeed ? -walledMaxGlidingSpeed : vel.y - walledGlidingSpeed;
            _rb.velocity = vel;
            return;
        }

        vel.z = vel.z > maxSpeed ? maxSpeed : vel.z;
        vel.z = vel.z < -maxSpeed ? -maxSpeed : vel.z;
        _rb.velocity = vel;

        if (!(_rb.velocity.y < 0)) return;
        _rb.velocity += Vector3.down * fallMultiplier;
    }

    private void OnWalk(InputAction.CallbackContext context)
    {
        _dir = context.ReadValue<Vector2>();
    }

    private void OnStop(InputAction.CallbackContext context)
    {
        _dir = Vector2.zero;
    }

    public void OnPlaymodeEnd()
    {
        walkMap.Disable();
        jumpMap.Disable();
        _rb.velocity = Vector3.zero;
    }
    public void OnPlaymodeStart()
    {
        walkMap.Enable();
        jumpMap.Enable();
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        if(_groundCheck.IsColliding)
            _rb.AddForce(new Vector3(0, jumpForce, 0));
        
        if(_leftWallCheck.IsColliding)
            _rb.AddForce(new Vector3(0, jumpForce, directionalJumpForce));
        
        if(_rightWallCheck.IsColliding)
            _rb.AddForce(new Vector3(0, jumpForce, -directionalJumpForce));
    }
}
