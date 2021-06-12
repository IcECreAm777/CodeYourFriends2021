using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragObject : MonoBehaviour
{
    private Vector2 mousePosition;

    private GameObject player;
    private MouseInputController mouseInputController;
    private bool mouseButtonDown;

    private float mZCoord;
    private Vector3 mOffset;

    private Vector3 mouseOffsetTest;

    private RaycastHit[] collidersUnderMouse;

    private Vector3 intersection;

    private bool dragOn;

    private void Start()
    {
        player = GameObject.Find("Capsule");
        mouseInputController = player.GetComponent<MouseInputController>();
        mouseButtonDown = mouseInputController.mouseButtonDown;
    }

    void Update()
    {
        mousePosition = mouseInputController.mousePosition;
        mouseButtonDown = mouseInputController.mouseButtonDown;
        collidersUnderMouse = mouseInputController.collidersUnderMouse;


        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        var factor = -(ray.origin.x) / ray.direction.x;

        intersection = ray.origin + factor * ray.direction;

        if (mouseButtonDown && collidersUnderMouse.Length > 0) dragOn = true;

        if (dragOn)
        {
            mouseDrag();
        }

        if (!mouseButtonDown) dragOn = false;
    }

    private void mouseDrag()
    {
        var currentPos = intersection;
        transform.position = new Vector3(Mathf.Round(currentPos.x), Mathf.Round(currentPos.y), Mathf.Round(currentPos.z));
    }
}