using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class LevelTile : MonoBehaviour
{
    //STATE
    private bool _isPlaced = false;
    private bool _locked = false;
    private bool _playmode = false;
    private Vector3 _grabOffset;
    private bool _first = true;

    //COMPONENTS
    // private Collider _collider;
    // private Renderer _renderer;
    //private MouseInputController mouseInputController;
    private GridManager _gridManager;


    private SpriteRenderer _texImage;

    // Start is called before the first frame update
    void Start()
    {
        // _collider = GetComponent<Collider>();
        // _renderer = GetComponent<Renderer>();

        _gridManager = FindObjectOfType<GridManager>();
;
        // OnPlaymodeStart(); //TODO: change depending on if we start in playmode or not

        UpdateTexture();
    }

    void UpdateTexture()
    {
        if(_texImage == null)
        {
            var plane = Instantiate(new GameObject(), transform);
            // var plane = new GameObject();
            plane.transform.parent = transform;
            plane.transform.localPosition = new Vector3(-1, 12.5f, 12.5f); //slightly behind
            plane.transform.localScale = new Vector3(5f, 5f, 1);
            plane.transform.localRotation = Quaternion.Euler(0, 90, 0);
            // plane.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1.0f); //transparent
            _texImage = plane.AddComponent<SpriteRenderer>();
            // _texImage.GetComponent<Collider>().enabled = false;
            _gridManager = FindObjectOfType<GridManager>();
        }

        if(_playmode)
        {
            _texImage.enabled = false;
            return;
        }
        _texImage.enabled = true;
        if(_locked)
            _texImage.sprite = _gridManager._texLocked;
        else if(_isPlaced)
            _texImage.sprite = _gridManager._texPlaced;
        else
            _texImage.sprite = _gridManager._texMovable;
        
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: remove
        if(_first && gameObject.name == "Fixed")
        {
            _first = false;
            ForcePlaceAt(14, 18);
        }
    }

    public void OnPlaymodeStart()
    {
        // _collider.enabled = _isPlaced;
        if(!_isPlaced)
        {
            Debug.Log("hiding " + gameObject.name);
            for(int i = 0; i < gameObject.transform.childCount; ++i)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        _playmode = true;
        UpdateTexture();
    }

    public void OnPlaymodeEnd()
    {
        // _collider.enabled = true;
        for(int i = 0; i < gameObject.transform.childCount; ++i)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
        _playmode = false;
        UpdateTexture();
    }

    public void SetPlaced(bool placed)
    {
        Debug.Log("set placed " + placed + " " + gameObject);
        _isPlaced = placed;
        // _renderer.material.color = placed ? Color.green : Color.gray;
        UpdateTexture();
    }

    public void LockTile()
    {
        _locked = true;
        // _renderer.material.color = Color.red;
        // _collider.enabled = true;
        for(int i = 0; i < gameObject.transform.childCount; ++i)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
        UpdateTexture();
    }

    // public void UnlockTile()
    // {
    //     _locked = false;
    //     // _renderer.material.color = _isPlaced ? Color.green : Color.gray;
    //     UpdateTexture();
    // }

    public bool IsTileLocked()
    {
        return _locked;
    }

    public bool StartDragging(Vector3 pos)
    {
        
#if DEBUG
        if(_locked || _playmode)
        {
            Debug.Log("You failed.");
            Destroy(gameObject);
            return false;
        }
#endif
        
        _gridManager.PositionToGridCoords(pos, out var x, out var y);
        if (!_gridManager.CanRemoveTile(x, y))
        {
            return false;
        }

        _grabOffset = pos - transform.position;
        if(_isPlaced) _gridManager.RemoveTile(x, y);
        return true;
    }

    public void Drag(Vector3 currentPos)
    {
        
#if DEBUG
        if (_locked)
        {
            Debug.Log("You fucked up");
            Destroy(gameObject);
        }
#endif
        
        _gridManager.PositionToGridCoords(currentPos, out var x, out var y);

        if(_gridManager.CanPlaceTile(x, y))
        {
            transform.position = _gridManager.GridCoordsToPosition(x, y);
        }
        else
        {
            transform.position = currentPos - _grabOffset;
        }
    }

    public void StopDragging(Vector3 currentPos)
    {
        _gridManager.PositionToGridCoords(currentPos, out var x, out var y);
        _gridManager.PlaceTile(x, y, this); //checks if we can place the tile
    }

    public void ForcePlaceAt(int x, int y)
    {
        // _renderer.material.color = Color.yellow;
        transform.position = _gridManager.GridCoordsToPosition(x, y);
        _gridManager.ForcePlaceTile(x, y, this);
        LockTile();
    }
}
