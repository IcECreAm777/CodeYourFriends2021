using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField]
    int gridExtendZ = 15;
    [SerializeField]
    int gridExtendY = 15;

    [SerializeField]
    int tileSize = 2;

    [Header("Images")]
    public Sprite _texPlaced;
    public Sprite _texLocked;
    public Sprite _texMovable;


    public LevelTile[,] _grid = null;

    // Start is called before the first frame update
    void Start()
    {
        InitGrid();
    }
    public void InitGrid()
    {
        if(_grid == null) _grid = new LevelTile[gridExtendZ*2, gridExtendY*2];
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PositionToGridCoords(Vector3 pos, out int x, out int y)
    {
        x = Mathf.FloorToInt(pos.z/tileSize) + gridExtendZ;
        y = Mathf.FloorToInt(pos.y/tileSize) + gridExtendY;
        // Debug.Log("pos = " + pos + "  x = " + x + "  y = " + y);
    }

    public Vector3 GridCoordsToPosition(int x, int y)
    {
        return tileSize * new Vector3(0, y-gridExtendY, x-gridExtendZ);
    }

    public bool CanRemoveTile(int x, int y)
    {
        // Debug.Log("CAN REMOVE " + x + " " + y + " ?");
        //check if by removing this tile, all neighbours are still connected to any locked tile
        if(x<(2*gridExtendZ-1) && !IsConnectedToLockedTile(x+1, y, x, y))
        {
            // Debug.Log("VISITED: false after 1st");
            return false;
        }
        if(y<(2*gridExtendY-1) && !IsConnectedToLockedTile(x, y+1, x, y))
        {
            // Debug.Log("VISITED: false after 2nd");
            return false;
        }
        if(x>0 && !IsConnectedToLockedTile(x-1, y, x, y))
        {
            // Debug.Log("VISITED: false after 3rd");
            return false;
        }
        if(y>0 && !IsConnectedToLockedTile(x, y-1, x, y))
        {
            // Debug.Log("VISITED: false after 4th");
            return false;
        }
        return true;
    }

    private bool IsConnectedToLockedTile(int x, int y, int ignoreX, int ignoreY)
    {
        // Debug.Log("START FROM ? " + x + " " + y);
        if(_grid[x, y] == null) return true;
        var visited = new bool[2*gridExtendY, 2*gridExtendZ];
        visited[ignoreX,ignoreY] = true; //this will ignore the tile in question
        return IsConnectedToLockedTile(x, y, visited);
    }

    private bool IsConnectedToLockedTile(int x, int y, bool[,] visited)
    {
        // Debug.Log("visited ? " + x + " " + y);
        if(_grid[x, y] == null) return false; //an empty space does not count as connection point
        // if(_grid[x, y].IsLocked()) Debug.Log("found a locked tile");
        if(_grid[x, y].IsTileLocked()) return true; //locked tiles are endpoints

        if(visited[x, y]) return false; //already visited this node, skip
        visited[x, y] = true;

        // if any of our neighbors are connected, we are too
        // check in top-right direction first
        if(x<(2*gridExtendZ-1) && IsConnectedToLockedTile(x+1, y, visited)) return true;
        if(y<(2*gridExtendY-1) && IsConnectedToLockedTile(x, y+1, visited)) return true;
        if(x>0 && IsConnectedToLockedTile(x-1, y, visited)) return true;
        if(y>0 && IsConnectedToLockedTile(x, y-1, visited)) return true;
        return false;
    }
    

    public bool CanPlaceTile(int x, int y)
    {
        //current cell is free
        if(_grid[x, y] != null) return false;

        //there is at least one neighbouring tile
        if(x>0 && _grid[x-1, y] != null) return true;
        if(y>0 && _grid[x, y-1] != null) return true;
        if(x<(2*gridExtendZ-1) && _grid[x+1, y] != null) return true;
        if(y<(2*gridExtendY-1) && _grid[x, y+1] != null) return true;

        return false;
    }

    public void ForcePlaceTile(int x, int y, LevelTile tile)
    {
        _grid[x, y] = tile;
        Debug.Log("PLACED: x:" + x + " y: " + y + " object: " + tile);
        tile.SetPlaced(true);
    }
    public void PlaceTile(int x, int y, LevelTile tile)
    {
        if(!CanPlaceTile(x, y))
        {
            Debug.Log("cannot place " + x + " " + y);
            return;
        }
        ForcePlaceTile(x, y, tile);
    }

    public void RemoveTile(int x, int y)
    {
        if(_grid[x, y] != null) _grid[x, y].SetPlaced(false);
        _grid[x, y] = null;
        Debug.Log("REMOVED: x:" + x + " y: " + y);
    }

}
