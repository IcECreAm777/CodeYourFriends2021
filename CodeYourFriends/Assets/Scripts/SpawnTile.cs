using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SpawnTile : MonoBehaviour
{
    [Header("Properties")] 
    [SerializeField]
    private int numTiles = 3;

    [Header("Game Objects")] 
    [SerializeField]
    private GameObject tileCollider;
    [Space] 
    [SerializeField]
    private List<TileCollection> tileCollections;

    private PlaymodeSwitch _playButton;

    public LevelTile _startTile;

    private void Start()
    {
        _playButton = FindObjectOfType<PlaymodeSwitch>();
        SpawnInitialTiles(_playButton.GetSpawnPosition());
    }

    public void SpawnTiles(Vector3 spawnPoint)
    {
        Debug.Log("SPAWN TILES");
        for (var i = 0; i < numTiles; i++)
        {
            var collectionIndex = Random.Range(0, tileCollections.Count);
            var tileIndex = Random.Range(0, tileCollections[collectionIndex].tiles.Count);
            var outer = Instantiate(tileCollider);
            Instantiate(tileCollections[collectionIndex].tiles[tileIndex].geometry, outer.transform, true);
            outer.transform.position = spawnPoint;
            var tileScript = outer.AddComponent<LevelTile>();
            _playButton.playmodeStartEvent.AddListener(tileScript.OnPlaymodeStart);
            _playButton.playmodeEndEvent.AddListener(tileScript.OnPlaymodeEnd);
        }
    }

    private void SpawnInitialTiles(Vector3 spawnPoint)
    {
        Debug.Log("SPAWN INITIAL TILES");
        var gm = FindObjectOfType<GridManager>();
        gm.InitGrid();
        int origX, origY;
        gm.PositionToGridCoords(spawnPoint, out origX, out origY);
        var origPos = gm.GridCoordsToPosition(origX, origY);

        //original spawn tile
        var spawn = Instantiate(tileCollider);
        Instantiate(tileCollections[0].tiles[0].geometry, spawn.transform, true);
        spawn.transform.position = origPos;
        _startTile = spawn.AddComponent<LevelTile>();
        _playButton.playmodeStartEvent.AddListener(_startTile.OnPlaymodeStart);
        _playButton.playmodeEndEvent.AddListener(_startTile.OnPlaymodeEnd);
        gm.ForcePlaceTile(origX, origY, _startTile);
        _startTile.LockTile();

        //teleport player
        FindObjectOfType<PlayerMovementBehaviour>().gameObject.transform.position = spawnPoint;
    }
}
