using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalReached : MonoBehaviour
{

    [Header("Spawn Settings")]
    [SerializeField]
    private float finishDistanceMultiplier = 10.0f;
    [SerializeField]
    private float finishDistanceOffset = 20;

    private Collider _playerCollider;
    private GridManager _gridManager;
    private PlaymodeSwitch _playSwitch;
    private int _finishCount = 0;
    private void Start()
    {
        _playerCollider = FindObjectOfType<PlayerMovementBehaviour>().gameObject.GetComponent<Collider>();
        _gridManager = FindObjectOfType<GridManager>();
        _playSwitch = FindObjectOfType<PlaymodeSwitch>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other != _playerCollider) return;
        Debug.Log(" ===== 1 ====");
        ++_finishCount;

        //set new spawn point to our current position
        // _playSwitch.spawnPoint.transform.position = transform.position;

        Debug.Log(" ===== 2 ====");
        //set our position to a random position a certain distance away
        var angle = UnityEngine.Random.Range(0, 360);
        float upper = finishDistanceMultiplier * _finishCount;
        float lower = upper * 2/3 ;
        var dist = UnityEngine.Random.Range(lower, upper) + finishDistanceOffset;
        var offset = Quaternion.Euler(angle, 0, 0) * (Vector3.up * dist);
        // transform.position += offset;
        gameObject.transform.position = _playSwitch.spawnPoint.transform.position + offset;
        Debug.Log(" ===== 3 ==== " + offset);

        var spawner = GetComponent<SpawnTile>();

        //move the initial start tile to now be the finish tile
        // int x, y;
        // _gridManager.PositionToGridCoords(transform.position, out x, out y);
        // _gridManager.PlaceTile(x, y, spawner._startTile);
        // spawner._startTile.transform.position = _gridManager.GridCoordsToPosition(x, y);
        // Debug.Log(" ===== 4 ====");

        // TODO get coordinates from grid
        spawner.SpawnTiles(new Vector3(0, 60, 0));
        FindObjectOfType<PlaymodeSwitch>().EditModeStart();
        Debug.Log(" ===== 5 ====");
    }
}
