using System;
using UnityEngine;

// Represents tiles with no unique functionality.
public class BasicTile : Tile{
    [SerializeField]
    // Used to find the tile info for the specific tile.
    private string tileName;

    // Finds the tile info for the specific tile.
    protected new void Awake(){
        base.Awake();
        tileInfo = Resources.Load("TileInfo/" + tileName + "Info") as TileInfo;
    }

    // Gets the ID using the tile name instead of the class name.
    public override TileID GetID(){ 
        foreach(TileID id in Enum.GetValues(typeof(TileID))){
            if(id.GetPrefabName() == tileName) return id;
        }
        return TileID.NULL_VALUE;
    }
}
