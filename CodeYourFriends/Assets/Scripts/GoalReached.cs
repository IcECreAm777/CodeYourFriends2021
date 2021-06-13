using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalReached : MonoBehaviour
{

    private Collider _playerCollider;
    private void Start()
    {
        _playerCollider = FindObjectOfType<PlayerMovementBehaviour>().gameObject.GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("==== 00");
        if(other != _playerCollider) return;
        Debug.Log("==== 01");
        // TODO get coordinates from grid
        GetComponent<SpawnTile>().SpawnTiles(new Vector3());
        Debug.Log("==== 02");
        var a = FindObjectOfType<PlaymodeSwitch>();
        Debug.Log("==== 03 " + a);
        a.EditModeStart();
        Debug.Log("==== 04");
    }
}
