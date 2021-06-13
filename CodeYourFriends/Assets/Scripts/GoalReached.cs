using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalReached : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
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
