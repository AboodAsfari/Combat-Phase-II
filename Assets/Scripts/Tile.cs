using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Tile : MonoBehaviour{
    protected TileInfo tileInfo;
    protected TileState tileState;
    protected SpriteInfo cliffsideSpriteInfo;
    protected SpriteInfo sideBorderSpriteInfo;

    private bool editing = false;
    private MapStateController gsc;

    protected void Awake(){
        tileInfo = Resources.Load("TileInfo/" + ObjectNames.GetClassName(this) + "Info") as TileInfo;
        tileState = ScriptableObject.CreateInstance<TileState>();
        if(GameObject.Find("Editor Controller") != null) gsc = GameObject.Find("Editor Controller").GetComponent<EditorController>().gsc;
        else gsc = GameObject.Find("Game Controller").GetComponent<GameController>().gsc;

        cliffsideSpriteInfo = Resources.Load("SpriteInfo/CliffsideSpriteInfo") as SpriteInfo;
        sideBorderSpriteInfo = Resources.Load("SpriteInfo/SideBorderSpriteInfo") as SpriteInfo;
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
        Transform borderContainer = transform.Find("Borders & Cliffside");
        GameObject cliffsideRight = borderContainer.Find("Right Cliffside").gameObject;
        GameObject cliffsideLeft = borderContainer.Find("Left Cliffside").gameObject;
        GameObject borderRight = borderContainer.Find("Right Border").gameObject;
        GameObject borderLeft = borderContainer.Find("Left Border").gameObject;
        GameObject borderTopLeft = borderContainer.Find("Top Left Border").gameObject;
        GameObject borderTopRight = borderContainer.Find("Top Right Border").gameObject;
        GameObject borderCornerTopLeft = borderContainer.Find("Top Left Corner Border").gameObject;
        GameObject borderCornerTopRight = borderContainer.Find("Top Right Corner Border").gameObject;
        GameObject borderCornerBottomRight = borderContainer.Find("Bottom Right Corner Border").gameObject;
        GameObject borderCornerBottomLeft = borderContainer.Find("Bottom Left Corner Border").gameObject;

        cliffsideRight.SetActive(ShowEdge(0, 1));
        cliffsideLeft.SetActive(ShowEdge(-1, 1));

        if(cliffsideRight.activeSelf){
            int rightLayers;
            if(GetAdjacent(0, 1) == null) rightLayers = tileState.GetElevation() + 2;
            else rightLayers = tileState.GetElevation() - GetElevation(0, 1);

            foreach(Transform child in cliffsideRight.transform){
                child.gameObject.SetActive(false);
            }
            cliffsideRight.transform.GetChild(rightLayers - 1).gameObject.SetActive(true);

            Transform cornerBorder = borderCornerBottomRight.transform;
            cornerBorder.localScale = new Vector3(1, cliffsideSpriteInfo.scaleY * rightLayers, 1);
            cornerBorder.localPosition = new Vector3(cornerBorder.localPosition.x, cliffsideSpriteInfo.initY 
                - (cliffsideSpriteInfo.height * (rightLayers - 1) * 0.5f), cornerBorder.localPosition.z); 
        }

        // TODO  

        if(cliffsideLeft.activeSelf){
            int leftLayers;
            if(GetAdjacent(-1, 1) == null) leftLayers = tileState.GetElevation() + 2;
            else leftLayers = tileState.GetElevation() - GetElevation(-1, 1);

            foreach(Transform child in cliffsideLeft.transform){
                child.gameObject.SetActive(false);
            }
            cliffsideLeft.transform.GetChild(leftLayers - 1).gameObject.SetActive(true);

            Transform cornerBorder = borderCornerBottomLeft.transform;
            cornerBorder.localScale = new Vector3(1, cliffsideSpriteInfo.scaleY * leftLayers, 1);
            cornerBorder.localPosition = new Vector3(cornerBorder.localPosition.x, cliffsideSpriteInfo.initY 
                - (cliffsideSpriteInfo.height * (leftLayers - 1) * 0.5f), cornerBorder.localPosition.z); 
        }

        borderRight.SetActive(ShowEdge(1, 0));
        borderLeft.SetActive(ShowEdge(-1, 0));
        borderTopLeft.SetActive(ShowEdge(0, -1));
        borderTopRight.SetActive(ShowEdge(1, -1));
        borderCornerTopLeft.SetActive(ShowEdge(-1, 0) && (ShowEdge(0, -1) || ShowEdgeLower(0, -1)));
        borderCornerTopRight.SetActive(ShowEdge(1, 0) && (ShowEdge(1, -1) || ShowEdgeLower(1, -1)));
        borderCornerBottomLeft.SetActive(ShowEdge(-1, 0) && ShowEdge(-1, 1));
        borderCornerBottomRight.SetActive(ShowEdge(1, 0) && ShowEdge(0, 1));

        borderCornerTopLeft.transform.Find("Pixel Fill").gameObject.SetActive(GetAdjacent(0, -1) == null || !ShowEdgeLower(0, -1));
        borderCornerTopRight.transform.Find("Pixel Fill").gameObject.SetActive(GetAdjacent(1, -1) == null || !ShowEdgeLower(1, -1));

        int extraTopLayers = 0;
        int diffBottomLayers = 0;
        if(GetAdjacent(1, -1) != null && GetElevation(1, -1) == tileState.GetElevation()) extraTopLayers = tileState.GetElevation() + 1;
        if(GetAdjacent(1, 0) != null && GetElevation(1, 0) < tileState.GetElevation()) extraTopLayers -= GetElevation(1, 0) + 2;
        if(GetAdjacent(0, 1) != null && GetElevation(0, 1) > tileState.GetElevation()) diffBottomLayers = GetElevation(0, 1) - tileState.GetElevation();
        borderRight.transform.localScale = new Vector3(1, sideBorderSpriteInfo.scaleY - ((extraTopLayers + diffBottomLayers) * sideBorderSpriteInfo.height), 1);
        borderRight.transform.localPosition = new Vector3(borderRight.transform.localPosition.x, sideBorderSpriteInfo.initY 
            - (extraTopLayers * sideBorderSpriteInfo.height * 0.5f) + (diffBottomLayers * sideBorderSpriteInfo.height * 0.5f), borderRight.transform.localPosition.z);

        // offsetTiles = 0;
        // if(GetAdjacent(0, -1) != null && GetElevation(0, -1) == tileState.GetElevation()) offsetTiles = tileState.GetElevation() + 1;
        // else if(GetAdjacent(1, 1) != null && GetElevation(1, 1) > tileState.GetElevation()) offsetTiles = (tileState.GetElevation() - GetElevation(1, 1)) * 0.5f;
        // if(offsetTiles < 0) borderLeft.transform.localScale = new Vector3(1, sideBorderSpriteInfo.scaleY + (offsetTiles * sideBorderSpriteInfo.height * 2), 1);
        // borderLeft.transform.localPosition = new Vector3(borderLeft.transform.localPosition.x, sideBorderSpriteInfo.initY 
        //     - (offsetTiles * sideBorderSpriteInfo.height), borderLeft.transform.localPosition.z);

        Tile GetAdjacent(int x, int y){ return gsc.GetTile(tileState.GetPosition() + new Vector2Int(x, y)); }
        int GetElevation(int x, int y){ return GetAdjacent(x, y).GetTileState().GetElevation(); }
        bool ShowEdge(int x, int y){ return GetAdjacent(x, y) == null || GetElevation(x, y) < tileState.GetElevation(); }
        bool ShowEdgeLower(int x, int y){ return GetAdjacent(x, y) == null || GetElevation(x, y) > tileState.GetElevation(); }
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

