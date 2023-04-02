using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTile : Tile{
    [SerializeField]
    private string tileName;

    protected new void Awake(){
        base.Awake();
        tileInfo = Resources.Load("TileInfo/" + tileName + "Info") as TileInfo;
    }

    public override TileID GetID(){ 
        foreach(TileID id in Enum.GetValues(typeof(TileID))){
            if(id.GetPrefabName() == tileName) return id;
        }
        return TileID.NULL_VALUE;
    }
}
