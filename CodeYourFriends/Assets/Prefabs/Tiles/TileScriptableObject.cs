using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile", menuName = "Tile/Tile")]
public class TileScriptableObject : ScriptableObject
{
    public GameObject geometry;
    
    // TODO add other properties like costs
}
