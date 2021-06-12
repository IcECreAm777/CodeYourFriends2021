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
    public RaycastHit[] collidersUnderMouse = new RaycastHit[10];

    void moveCursor(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
        var ray = Camera.main.ScreenPointToRay(mousePosition);
        collidersUnderMouse= Physics.RaycastAll(ray, Mathf.Infinity);
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
            action.performed += leftClick;
        }
    }

    private void Update()
    {
        var projectedMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        //Debug.Log(projectedMousePosition);
    }

}
