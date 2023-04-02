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
        else gsc = GameObject.Find("Game Controller").GetComponent<GameController>().gsc;
    }

    public void OnMouseOver(){
        if(Input.GetMouseButton(1) && editing && gsc.GetTile(tileState.GetPosition()) != null){
            gsc.DeleteTile(tileState.GetPosition());
        }
    }

    public void SetEditing(bool editing){ this.editing = editing; }

    public TileInfo GetTileInfo(){ return tileInfo; }

    public TileState GetTileState(){ return tileState; }

    public void UpdateVisual(){
        GameObject dirtRight = transform.Find("Right Dirt").gameObject;
        GameObject dirtLeft = transform.Find("Left Dirt").gameObject;
        GameObject borderRight = transform.Find("Right Border").gameObject;
        GameObject borderLeft = transform.Find("Left Border").gameObject;
        GameObject borderTopLeft = transform.Find("Top Left Border").gameObject;
        GameObject borderTopRight = transform.Find("Top Right Border").gameObject;
        GameObject borderBottomLeft = transform.Find("Bottom Left Border").gameObject;
        GameObject borderBottomRight = transform.Find("Bottom Right Border").gameObject;

        // ADD HEIGHT BASED FOR THESE
        dirtRight.SetActive(gsc.GetTile(tileState.GetPosition() + new Vector2Int(0, 1)) == null);
        dirtLeft.SetActive(gsc.GetTile(tileState.GetPosition() + new Vector2Int(-1, 1)) == null);
        
        borderRight.SetActive(gsc.GetTile(tileState.GetPosition() + new Vector2Int(1, 0)) == null);
        borderLeft.SetActive(gsc.GetTile(tileState.GetPosition() + new Vector2Int(-1, 0)) == null);
        borderTopLeft.SetActive(gsc.GetTile(tileState.GetPosition() + new Vector2Int(0, -1)) == null);
        borderTopRight.SetActive(gsc.GetTile(tileState.GetPosition() + new Vector2Int(1, -1)) == null);
        borderBottomLeft.SetActive(gsc.GetTile(tileState.GetPosition() + new Vector2Int(-1, 1)) == null);
        borderBottomRight.SetActive(gsc.GetTile(tileState.GetPosition() + new Vector2Int(0, 1)) == null);
    }

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
