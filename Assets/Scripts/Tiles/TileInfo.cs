using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileInfo", menuName = "Entity Info/Tile Info")]
// Static information about a tile.
public class TileInfo : ScriptableObject{
    // The name of the tile.
    [SerializeField]
    private String tileName;

    // Getters.
    public String GetTileName(){ return tileName; }
}
