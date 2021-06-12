using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragObject : MonoBehaviour
{
    private Vector2 mousePosition;

    private GameObject player;
    private MouseInputController mouseInputController;

    private void Start()
    {
        player = GameObject.Find("Capsule");
        mouseInputController = player.GetComponent<MouseInputController>();
    }

    void OnLeftButton(InputAction.CallbackContext context)
    {
        mouseInputController.leftClick(context);
        //Debug.Log("Click");
    }

    void Update()
    {
        var projectedMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        mousePosition = mouseInputController.mousePosition;
        //Debug.Log(mousePosition);
    }
}



