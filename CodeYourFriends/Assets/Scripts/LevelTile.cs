using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
    //STATE
    private bool _isPlaced = false;
    private bool _locked = false;
    private bool _dragging = false;
    private Vector3 _grabOffset;
    private bool _lastMouseDown = false;

    //COMPONENTS
    private Collider _collider;
    private Renderer _renderer;
    private MouseInputController mouseInputController;
    private GridManager gridManager;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<Renderer>();

        var player = GameObject.Find("Capsule");
        mouseInputController = player.GetComponent<MouseInputController>();

        gridManager = FindObjectOfType<GridManager>();

    }

    // Update is called once per frame
    private bool first = true;
    void Update()
    {
        if(first && gameObject.name == "Fixed")
        {
            first = false;
            Place();
        }

        var mouseDown = mouseInputController.mouseButtonDown;
        if(mouseDown != _lastMouseDown)
        {
            _lastMouseDown = mouseDown;
            if(mouseDown) MousePressed();
            else MouseReleased();
        }

        if (_dragging)
            MouseDrag();
    }

    void OnPlaymodeStart()
    {
        _collider.enabled = _isPlaced;
        _renderer.enabled = _isPlaced;
    }

    void OnPlaymodeEnd()
    {
        _collider.enabled = true;
        _renderer.enabled = true;
    }

    public void SetPlaced(bool placed)
    {
        _isPlaced = placed;
        _renderer.material.color = placed ? Color.green : Color.gray;
    }

    public void Lock()
    {
        _locked = true;
        _renderer.material.color = Color.red;
    }

    public void Unlock()
    {
        _locked = false;
        _renderer.material.color = _isPlaced ? Color.green : Color.gray;
    }

    public bool IsLocked()
    {
        return _locked;
    }

    private void MouseReleased()
    {
        if(_locked) return;
        if(! _dragging) return;

        _dragging = false;
        _renderer.material.color = Color.gray;

        //drop object into grid
        var currentPos = CurrentPos();
        int x, y;
        gridManager.PositionToGridCoords(currentPos, out x, out y);
        gridManager.PlaceTile(x, y, this); //checks if we can place the tile
    }

    private void MousePressed()
    {
        if(_locked) return;

        foreach(var c in mouseInputController.collidersUnderMouse)
        {
            if(c.collider.gameObject.GetComponent<LevelTile>() == null) continue;

            //we found the first LevelTile. check if it's us
            if(c.collider != _collider) break;

            //user clicked on our own collider
            var currentPos = CurrentPos();
            int x, y;
            gridManager.PositionToGridCoords(currentPos, out x, out y);
            if(_isPlaced && !gridManager.CanRemoveTile(x, y)) break;

            _dragging = true;
            _renderer.material.color = Color.white;

            _grabOffset = currentPos - transform.position;

            if(_isPlaced) gridManager.RemoveTile(x, y);

            break;
        }
    }

    private void MouseDrag()
    {
        if(_locked) return;

        var currentPos = CurrentPos();
        int x, y;
        gridManager.PositionToGridCoords(currentPos, out x, out y);

        if(gridManager.CanPlaceTile(x, y))
        {
            transform.position = gridManager.GridCoordsToPosition(x, y);
        }
        else
        {
            transform.position = currentPos - _grabOffset;
        }
    }

    private Vector3 CurrentPos()
    {
        var ray = Camera.main.ScreenPointToRay(mouseInputController.mousePosition);
        var factor = -(ray.origin.x) / ray.direction.x;
        return ray.origin + factor * ray.direction;
    }

    public void Place()
    {
        _renderer.material.color = Color.yellow;
        int x = 14;
        int y = 18;
        transform.position = gridManager.GridCoordsToPosition(x, y);
        gridManager.ForcePlaceTile(x, y, this);
        Lock();
    }
}
