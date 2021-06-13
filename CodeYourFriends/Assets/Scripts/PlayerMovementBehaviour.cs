using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
using Random = System.Random;

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

    [Header("Input Actions")]
    [SerializeField]
    private InputActionMap walkMap;
    [SerializeField]
    private InputActionMap jumpMap;
    [SerializeField]
    private InputActionMap camMoveMap;
    [SerializeField]
    private InputActionMap restartLevelMap;
    [SerializeField]
    private InputActionMap enableEditModeMap;
    [SerializeField]
    private InputAction debugAction;

    [Header("Camera Control")] 
    [SerializeField]
    private float timeToZoom = 1.0f;
    [SerializeField]
    private float cameraEditModeSpeed = 0.5f;
    [SerializeField]
    private GameObject editModePos;
    [SerializeField]
    private GameObject playModePos;
    
    [Header("Event settings")]
    [SerializeField]
    private BroadcastUfoEvent playModeStarted;
    [SerializeField]
    private PopupScriptableObject playModeStartedMessage;

    // non editor properties
    private Vector2 _dir;
    private bool _editMode = true;

    private bool _firstPlayMode = true;
    private List<InputActionMap> _playModeInputMaps;
    private List<InputActionMap> _editModeMaps;

    private PlaymodeSwitch _start;

    // components
    private Rigidbody _rb;
    private CapsuleCollider _collider;
    private Camera _cam;
    
    // children 
    private GetCollisionScript _groundCheck;
    private GetCollisionScript _leftWallCheck;
    private GetCollisionScript _rightWallCheck;

    protected void Awake()
    {
        _start = FindObjectOfType<PlaymodeSwitch>();
        
        _playModeInputMaps = new List<InputActionMap>()
        {
            walkMap, jumpMap, restartLevelMap, enableEditModeMap
        };

        _editModeMaps = new List<InputActionMap>()
        {
            camMoveMap
        };

        debugAction.Enable();
        debugAction.performed += debug;

        foreach (var action in walkMap)
        {
            action.performed += OnWalk;
            action.canceled += OnStop;
        }

        foreach (var action in jumpMap)
        {
            action.performed += OnJump;
        }

        foreach (var action in camMoveMap)
        {
            action.performed += OnWalk;
            action.canceled += OnStop;
        }

        foreach (var action in restartLevelMap)
        {
            action.performed += OnRestart;
        }

        foreach (var action in enableEditModeMap)
        {
            action.performed += OnEnableEditMode;
        }

        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();

        _groundCheck = transform.Find("GroundCheck").GetComponent<GetCollisionScript>();
        _leftWallCheck = transform.Find("LeftWallCheck").GetComponent<GetCollisionScript>();
        _rightWallCheck = transform.Find("RightWallCheck").GetComponent<GetCollisionScript>();

        _cam = Camera.main;

        OnPlaymodeStart();
    }

    protected void FixedUpdate()
    {
        if (_editMode)
        {
            EditModeUpdates();
            return;
        }
        
        PlayModeUpdates();
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
        _rb.velocity = Vector3.zero;
        _rb.useGravity = false;
        walkMap.Disable();
        jumpMap.Disable();
        StartCoroutine(Zoom(editModePos.transform, true));
        ToggleModeInputs(false);
        transform.GetComponentInChildren<UfoBehaviour>().StopPointingToGoal();
    }
    
    public void OnPlaymodeStart()
    {
        _rb.useGravity = true;
        StartCoroutine(Zoom(playModePos.transform, false));
        ToggleModeInputs(true);
        transform.GetComponentInChildren<UfoBehaviour>().StartPointingToGoal();
        RestartLevel();

        if(!_firstPlayMode) return;
        playModeStarted.Invoke(playModeStartedMessage);
        _firstPlayMode = false;
    }

    public void RestartLevel()
    {
        //TODO make it as coroutine and fade out
        transform.position = _start.GetSpawnPosition();
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

    private void OnRestart(InputAction.CallbackContext context)
    {
        RestartLevel();
    }

    private void OnEnableEditMode(InputAction.CallbackContext context)
    {
        _start.EditModeStart();
    }

    private IEnumerator Zoom(Transform targetTransform, bool editMode)
    {
        DisableAllInput();
        
        var time = 0.0f;
        var camTransform = _cam.transform;
        var currentCamPos = camTransform.position;
        var currentCamRot = camTransform.rotation;
        
        while (time < timeToZoom)
        {
            camTransform.position = Vector3.Lerp(currentCamPos, targetTransform.position, 1 / timeToZoom * time);
            camTransform.rotation = Quaternion.Lerp(currentCamRot, targetTransform.rotation, 1 / timeToZoom * time);
            time += Time.deltaTime;
            yield return null;
        }

        _editMode = editMode;
        yield return null;
    }

    private void PlayModeUpdates()
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

    private void EditModeUpdates()
    {
        var camTransform = _cam.transform;
        var newPos = camTransform.position;
        newPos.y += _dir.y * cameraEditModeSpeed; 
        newPos.z += _dir.x * cameraEditModeSpeed;
        camTransform.position = newPos;
    }

    private void ToggleModeInputs(bool playMode)
    {
        if (playMode)
        {
            foreach (var playModeInputMap in _playModeInputMaps)
            {
                playModeInputMap.Enable();
            }

            foreach (var editModeMap in _editModeMaps)
            {
                editModeMap.Disable();
            }
            
            return;
        }
        
        foreach (var playModeInputMap in _playModeInputMaps)
        {
            playModeInputMap.Disable();
        }

        foreach (var editModeMap in _editModeMaps)
        {
            editModeMap.Enable();
        }
    }

    private void DisableAllInput()
    {
        foreach (var playModeInputMap in _playModeInputMaps)
        {
            playModeInputMap.Disable();
        }

        foreach (var editModeMap in _editModeMaps)
        {
            editModeMap.Disable();
        }
    }

    private void debug(InputAction.CallbackContext context)
    {
        //OnPlaymodeEnd();
        var yes = GetComponentInChildren<UfoBehaviour>();
        yes.BroadcastPopupMessage(playModeStartedMessage.message, 5.0f);
    }
}
