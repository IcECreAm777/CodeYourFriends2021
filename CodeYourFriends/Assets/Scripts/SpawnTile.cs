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

    private void Start()
    {
        _playButton = FindObjectOfType<PlaymodeSwitch>();
        SpawnInitialTiles();
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

    private void SpawnInitialTiles()
    {
        Debug.Log("SPAWN INITIAL TILES");
        var gm = FindObjectOfType<GridManager>();
        gm.InitGrid();
        int origX, origY;
        gm.PositionToGridCoords(Vector3.zero, out origX, out origY);
        var origPos = gm.GridCoordsToPosition(origX, origY);

        //original spawn tile
        var spawn = Instantiate(tileCollider);
        Instantiate(tileCollections[0].tiles[0].geometry, spawn.transform, true);
        spawn.transform.position = origPos;
        var tileScript = spawn.AddComponent<LevelTile>();
        _playButton.playmodeStartEvent.AddListener(tileScript.OnPlaymodeStart);
        _playButton.playmodeEndEvent.AddListener(tileScript.OnPlaymodeEnd);
        gm.ForcePlaceTile(origX, origY, tileScript);
        tileScript.LockTile();

        //teleport player
        FindObjectOfType<PlayerMovementBehaviour>().gameObject.transform.position = origPos + new Vector3(0, 10, 10);
    }
}
