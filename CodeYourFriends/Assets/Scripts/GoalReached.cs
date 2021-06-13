using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalReached : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject.Find("PlaymodeButton").GetComponent<PlaymodeSwitch>().EditModeStart();
        // TODO get coordinates from grid
        GetComponent<SpawnTile>().SpawnTiles(new Vector3());
    }
}
