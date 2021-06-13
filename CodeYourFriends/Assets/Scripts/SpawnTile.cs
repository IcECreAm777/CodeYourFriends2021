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
    }

    public void SpawnTiles(Vector3 spawnPoint)
    {
        Debug.Log("SPAWN TILES");
        for (var i = 0; i < numTiles; i++)
        {
            var collectionIndex = Random.Range(0, tileCollections.Count);
            var tileIndex = Random.Range(0, tileCollections[collectionIndex].tiles.Count);
            var outer = Instantiate(tileCollider);
            Instantiate(tileCollections[0].tiles[tileIndex].geometry, outer.transform, true);
            outer.transform.position = spawnPoint;
            var tileScript = outer.AddComponent<LevelTile>();
            _playButton.playmodeStartEvent.AddListener(tileScript.OnPlaymodeStart);
            _playButton.playmodeEndEvent.AddListener(tileScript.OnPlaymodeEnd);
        }
    }
}
