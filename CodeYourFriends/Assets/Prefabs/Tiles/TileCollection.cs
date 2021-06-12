using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Tile Collection", menuName = "Tile/Collection")]
public class TileCollection : ScriptableObject
{
    public List<TileScriptableObject> tiles;
}
