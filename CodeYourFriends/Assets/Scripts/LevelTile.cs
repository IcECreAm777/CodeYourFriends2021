using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
    //STATE
    private bool _isPlaced = false;
    private bool _locked = false;
    private bool _dragging = false;
    private bool _playmode = false;
    private Vector3 _grabOffset;
    private bool _lastMouseDown = false;

    //COMPONENTS
    // private Collider _collider;
    // private Renderer _renderer;
    private MouseInputController mouseInputController;
    private GridManager gridManager;

    // Start is called before the first frame update
    void Start()
    {
        // _collider = GetComponent<Collider>();
        // _renderer = GetComponent<Renderer>();

        var player = GameObject.Find("Capsule");
        mouseInputController = player.GetComponent<MouseInputController>();

        gridManager = FindObjectOfType<GridManager>();

        OnPlaymodeStart(); //TODO: change depending on if we start in playmode or not
    }

    // Update is called once per frame
    private bool first = true;
    void Update()
    {
        //TODO: remove
        if(first && gameObject.name == "Fixed")
        {
            first = false;
            ForcePlaceAt(14, 18);
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

    public void OnPlaymodeStart()
    {
        MouseReleased(); //drop tile if dragging
        // _collider.enabled = _isPlaced;
        if(!_isPlaced)
        {
            Debug.Log("hiding " + gameObject);
            for(int i = 0; i < gameObject.transform.childCount; ++i)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }


        _playmode = true;
    }

    public void OnPlaymodeEnd()
    {
        // _collider.enabled = true;
        for(int i = 0; i < gameObject.transform.childCount; ++i)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
        _playmode = false;
    }

    public void SetPlaced(bool placed)
    {
        Debug.Log("set placed " + placed + " " + gameObject);
        _isPlaced = placed;
        // _renderer.material.color = placed ? Color.green : Color.gray;
    }

    public void Lock()
    {
        _locked = true;
        // _renderer.material.color = Color.red;
        // _collider.enabled = true;
        for(int i = 0; i < gameObject.transform.childCount; ++i)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void Unlock()
    {
        _locked = false;
        // _renderer.material.color = _isPlaced ? Color.green : Color.gray;
    }

    public bool IsLocked()
    {
        return _locked;
    }

    private void MouseReleased()
    {
        if(_locked) return;
        if(_playmode) return;
        if(! _dragging) return;

        _dragging = false;
        // _renderer.material.color = Color.gray;

        //drop object into grid
        var currentPos = CurrentPos();
        int x, y;
        gridManager.PositionToGridCoords(currentPos, out x, out y);
        gridManager.PlaceTile(x, y, this); //checks if we can place the tile
    }

    private void MousePressed()
    {
        if(_locked) return;
        if(_playmode) return;

        foreach(var c in mouseInputController.collidersUnderMouse)
        {
            if(c.collider.gameObject.GetComponent<LevelTile>() == null) continue;

            //we found the first LevelTile. check if it's us
            // if(c.collider != _collider) break;
            if(c.collider != GetComponent<Collider>()) break;

            //user clicked on our own collider
            var currentPos = CurrentPos();
            int x, y;
            gridManager.PositionToGridCoords(currentPos, out x, out y);
            if(_isPlaced && !gridManager.CanRemoveTile(x, y)) break;

            _dragging = true;
            // _renderer.material.color = Color.white;

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

    public void ForcePlaceAt(int x, int y)
    {
        // _renderer.material.color = Color.yellow;
        transform.position = gridManager.GridCoordsToPosition(x, y);
        gridManager.ForcePlaceTile(x, y, this);
        Lock();
    }
}
