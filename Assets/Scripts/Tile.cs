using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Tile : MonoBehaviour{
    protected TileInfo tileInfo;
    protected TileState tileState;

    private bool editing = false;
    private MapStateController gsc;

    protected void Awake(){
        tileInfo = Resources.Load("TileInfo/" + ObjectNames.GetClassName(this) + "Info") as TileInfo;
        tileState = ScriptableObject.CreateInstance<TileState>();
        if(GameObject.Find("Editor Controller") != null) gsc = GameObject.Find("Editor Controller").GetComponent<EditorController>().gsc;
    }

    public void OnMouseOver(){
        if(Input.GetMouseButton(1) && editing && gsc.GetTile(tileState.GetPosition()) != null){
            gsc.DeleteTile(tileState.GetPosition());
        }
    }

    public void SetEditing(bool editing){ this.editing = editing; }

    public TileInfo GetTileInfo(){ return tileInfo; }

    public TileState GetTileState(){ return tileState; }

    public virtual TileID GetID(){ 
        foreach(TileID id in Enum.GetValues(typeof(TileID))){
            if(id.GetPrefabName() == ObjectNames.GetClassName(this)) return id;
        }
        return TileID.NULL_VALUE;
    }
}

public enum TileID{
    GRASS_TILE,
    NULL_VALUE
}

public static class TileExtensions{
    public static String GetPrefabName(this TileID type){
        switch(type){
            case TileID.GRASS_TILE : return "GrassTile";
        }
        return null;
    }
}
