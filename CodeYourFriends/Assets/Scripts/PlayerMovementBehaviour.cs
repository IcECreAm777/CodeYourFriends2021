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
    [SerializeField]
    public DeathPlane deathPlane;
    [SerializeField]
    public GridManager gridManager;

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
    [SerializeField]
    private InputActionMap mouseMovementMap;
    [SerializeField]
    private InputActionMap clickMap;

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
    
    private Vector2 _mousePosition;
    private bool _mouseButtonDown;
    private RaycastHit[] _collidersUnderMouse = new RaycastHit[10];
    private LevelTile _draggedTile;

    // components
    private Rigidbody _rb;
    private Camera _cam;

    // children 
    private GetCollisionScript _groundCheck;
    private GetCollisionScript _leftWallCheck;
    private GetCollisionScript _rightWallCheck;
    
    // animations
    private Animator _animations;
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int Jumping = Animator.StringToHash("Jumping");
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int Walled = Animator.StringToHash("Walled");
    private static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");

    protected void Awake()
    {
        _start = FindObjectOfType<PlaymodeSwitch>();
        _animations = GetComponentInChildren<Animator>();

        _playModeInputMaps = new List<InputActionMap>
        {
            walkMap, jumpMap, restartLevelMap, enableEditModeMap
        };

        _editModeMaps = new List<InputActionMap>
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
        
        foreach (var action in mouseMovementMap)
        {
            action.performed += MoveCursor;
        }
        foreach (var action in clickMap)
        {
            action.performed += OnClick;
            action.canceled += OnRelease;
        }

        mouseMovementMap.Enable();
        clickMap.Enable();
        
        _rb = GetComponent<Rigidbody>();

        _groundCheck = transform.Find("GroundCheck").GetComponent<GetCollisionScript>();
        _leftWallCheck = transform.Find("LeftWallCheck").GetComponent<GetCollisionScript>();
        _rightWallCheck = transform.Find("RightWallCheck").GetComponent<GetCollisionScript>();

        _cam = Camera.main;
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
        StartCoroutine(Zoom(editModePos.transform, true));
        ToggleModeInputs(false);
        transform.GetComponentInChildren<UfoBehaviour>().StopPointingToGoal();
    }
    
    public void OnPlaymodeStart()
    {
        if(_draggedTile != null)
            _draggedTile.StopDragging(MouseToWorldPos());
        
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
        deathPlane.deathUI.SetActive(false);
        //TODO make it as coroutine and fade out

        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        SpawnDeathPlane();

        transform.position = _start.GetSpawnPosition();
    }


    private void SpawnDeathPlane()
    {
        LevelTile[,] grid = gridManager._grid;

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (!System.Object.Equals(grid[i, j],null))
                {
                    Debug.Log("Position has been adjusted");
                    var newPosition = gridManager.GridCoordsToPosition(i, j);
                    newPosition.y -= 25;
                    Debug.Log("New position adjusted");
                    deathPlane.transform.position = newPosition;
                }
                else continue;
            }
        }
    }
    
    private void OnJump(InputAction.CallbackContext context)
    {
        if (_groundCheck.IsColliding)
        {
            _rb.AddForce(new Vector3(0, jumpForce, 0));
            _animations.SetBool(Jumping, true);
        }

        if (_leftWallCheck.IsColliding)
        {
            _rb.AddForce(new Vector3(0, jumpForce, directionalJumpForce));
            _animations.SetBool(Jumping, true);
        }

        if (_rightWallCheck.IsColliding)
        {
            _rb.AddForce(new Vector3(0, jumpForce, -directionalJumpForce));
            _animations.SetBool(Jumping, true);
        }
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
            UpdateAnimator();
            return;
        }

        vel.z = vel.z > maxSpeed ? maxSpeed : vel.z;
        vel.z = vel.z < -maxSpeed ? -maxSpeed : vel.z;
        _rb.velocity = vel;
        
        UpdateAnimator();

        if (!(_rb.velocity.y < 0)) return;
        var velocity = _rb.velocity;
        velocity += Vector3.down * fallMultiplier;
        _rb.velocity = velocity;
    }

    private void UpdateAnimator()
    {
        var vel = _rb.velocity;
        var runningLeft = vel.z < -0.1f;
        var runningRight = vel.z > 0.1f;
        
        if (runningLeft && !runningRight)
        {
            var newRot = Quaternion.Euler(0, 180, 0);
            _animations.transform.rotation = newRot;
        }

        if (runningRight && !runningLeft)
        {
            var newRot = Quaternion.Euler(0, 0, 0);
            _animations.transform.rotation = newRot;
        }
        
        _animations.SetFloat(VerticalSpeed, vel.y);
        _animations.SetBool(Running, runningLeft || runningRight);
        _animations.SetBool(Grounded, _groundCheck.IsColliding);
        _animations.SetBool(Walled, _leftWallCheck.IsColliding || _rightWallCheck.IsColliding);
        _animations.SetBool(Jumping, false);
    }

    private void EditModeUpdates()
    {
        if (_mouseButtonDown && _draggedTile != null)
        {
            _draggedTile.Drag(MouseToWorldPos());
            return;
        }
        
        //Updating cam (girls)
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
    
    private void MoveCursor(InputAction.CallbackContext context)
    {
        _mousePosition = context.ReadValue<Vector2>();
        var ray = _cam.ScreenPointToRay(_mousePosition);
        _collidersUnderMouse= Physics.RaycastAll(ray, Mathf.Infinity);
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        _mouseButtonDown = true;

        foreach (var raycastHit in _collidersUnderMouse)
        {
            var tile = raycastHit.collider.gameObject.GetComponent<LevelTile>();
            if(tile == null || tile.IsTileLocked()) continue;
            _draggedTile = tile.StartDragging(MouseToWorldPos()) ? tile : null;
            break;
        }
    }
    private void OnRelease(InputAction.CallbackContext context)
    {
        _mouseButtonDown = false;
        
        if(_draggedTile != null)  
            _draggedTile.StopDragging(MouseToWorldPos());

        _draggedTile = null;
    }

    private Vector3 MouseToWorldPos()
    {
        var ray = _cam.ScreenPointToRay(_mousePosition);
        var factor = -(ray.origin.x) / ray.direction.x;
        return ray.origin + factor * ray.direction;
    }

    private void debug(InputAction.CallbackContext context)
    {
        //OnPlaymodeEnd();
        var yes = GetComponentInChildren<UfoBehaviour>();
        yes.BroadcastPopupMessage(playModeStartedMessage.message, 5.0f);
    }
}
