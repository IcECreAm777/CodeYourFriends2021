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

    public void SpawnTiles(Vector3 spawnPoint)
    {
        for (var i = 0; i < numTiles; i++)
        {
            var collectionIndex = Random.Range(0, tileCollections.Count);
            var tileIndex = Random.Range(0, tileCollections[collectionIndex].tiles.Count);
            var wrapper = new GameObject("Tile");
            Instantiate(tileCollections[0].tiles[tileIndex].geometry, wrapper.transform, true);
            Instantiate(tileCollider, wrapper.transform, true);
            wrapper.transform.position = spawnPoint;
        }
    }
}
