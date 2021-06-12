using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInputController : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField]
    private InputActionMap mouseMovementMap;

    [SerializeField]
    private InputActionMap clickMap;

    public Vector2 mousePosition;
    public bool mouseButtonDown;

    void moveCursor(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }

    public void leftClick(InputAction.CallbackContext context)
    {
        Debug.Log("Click");
    }

    private void Awake()
    {
        mouseMovementMap.Enable();
        clickMap.Enable();
        foreach (var action in mouseMovementMap)
        {
            action.performed += moveCursor;
        }
        foreach (var action in clickMap)
        {
            action.performed += OnClick;
            action.canceled += OnRelease;
        }
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        mouseButtonDown = true;
        //Debug.Log("Click");
    }
    private void OnRelease(InputAction.CallbackContext context)
    {
        mouseButtonDown = false;
        //Debug.Log("Release");
    }

    private void Update()
    {
        //var projectedMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        //Debug.Log(projectedMousePosition);
    }

}
