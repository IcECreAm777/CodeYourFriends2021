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

    }
    private void OnMouseDown()
    {
        mOffset = gameObject.transform.position - GetMouseWorldPos();
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = mousePosition;
        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);

    }
    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + mOffset;
    }
}



