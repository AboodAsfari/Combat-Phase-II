using System;
using UnityEditor;
using UnityEngine;

// Base class for all tiles.
public class Tile : MonoBehaviour{
    // Information about the tile.
    [SerializeField]
    protected TileInfo tileInfo;
    protected TileState tileState;

    // Information about the tile sprites.
    protected SpriteInfo cliffsideSpriteInfo;
    protected SpriteInfo sideBorderSpriteInfo;

    // Tracks whether the map editor is currently open.
    private bool editing = false;

    // Tracks whether or not the tile is being hovered.
    private bool isHover = false;

    // The object that controls the current map, whether it's an editor or game.
    private MapStateController msc;

    // References to either the editor controller or the game controller,
    // one of these will always be null.
    private EditorController editorController;
    private GameController gameController;

    // Loads map state controller, as well as tile information
    // and tile sprite info.
    protected void Awake(){
        if(GameObject.Find("Editor Controller") != null){
            editorController = GameObject.Find("Editor Controller").GetComponent<EditorController>(); 
            msc = editorController.msc;
        }else{
            gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
            msc = gameController.msc;
        }

        tileState = ScriptableObject.CreateInstance<TileState>();

        cliffsideSpriteInfo = Resources.Load("SpriteInfo/CliffsideSpriteInfo") as SpriteInfo;
        sideBorderSpriteInfo = Resources.Load("SpriteInfo/SideBorderSpriteInfo") as SpriteInfo;
    }

    // Deletes a tile if the editor is open and the right mouse button 
    // is pressed over the tile.
    private void OnMouseOver(){
        if(Input.GetMouseButton(1) && editing && msc.GetTile(tileState.GetPosition()) != null){
            msc.DeleteTile(tileState.GetPosition());
        }

        if(Input.GetMouseButton(1) && !editing && gameController.IsInTileOverride()){
            gameController.ExitTileOverride();
        }

        if(Input.GetMouseButton(2)){
            transform.Find("Tile Hover").gameObject.SetActive(false);
            if(msc.GetUnit(tileState.GetPosition()) != null) msc.GetUnit(tileState.GetPosition()).SetHover(false);
            return;
        }else if(!isHover){
            isHover = true;
            if(msc.GetUnit(tileState.GetPosition()) != null) msc.GetUnit(tileState.GetPosition()).SetHover(true);
            else transform.Find("Tile Hover").gameObject.SetActive(true);
        }
    }
    
    // Selects an entity on the tile, the priority order is unit > building > tile.
    private void OnMouseDown(){
        if(editing) return;
        if(gameController.IsInTileOverride()){
            if(transform.Find("Tile Override").gameObject.activeSelf){
                Action<Vector2Int> action = gameController.GetOverrideFunction();
                gameController.ExitTileOverride();
                gameController.ResetSelect();
                action(GetTileState().GetPosition());
            }else{
                gameController.ResetSelect();
                gameController.ExitTileOverride();
                if(msc.GetUnit(tileState.GetPosition()) != null) gameController.SelectEntity(msc.GetUnit(tileState.GetPosition()));
                else gameController.SelectEntity(GetComponent<Tile>());
            }
        }else{
            if(msc.GetUnit(tileState.GetPosition()) != null) gameController.SelectEntity(msc.GetUnit(tileState.GetPosition()));
            else gameController.SelectEntity(GetComponent<Tile>());
        }
    }

    // Turns off the hover visual for a tile when the mouse 
    // is no longer over it.
    private void OnMouseExit(){
        isHover = false;
        transform.Find("Tile Hover").gameObject.SetActive(false);
        if(msc.GetUnit(tileState.GetPosition()) != null) msc.GetUnit(tileState.GetPosition()).SetHover(false);
    }

    // Setters.
    public void SetEditing(bool editing){ this.editing = editing; }

    // Getters.
    public TileInfo GetTileInfo(){ return tileInfo; }
    public TileState GetTileState(){ return tileState; }

    // Updates the visual of the tile based on surrounding
    // tiles, this affects border and cliffside height.
    public void UpdateVisual(){
        // Gets all relevant border game objects.
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

        // Sets all border based on the status of the relevant
        // adjacent tiles.
        cliffsideRight.SetActive(ShowEdge(Direction.BOTTOM_RIGHT));
        cliffsideLeft.SetActive(ShowEdge(Direction.BOTTOM_LEFT));

        SetCliffsideLayers(cliffsideRight, borderCornerBottomRight, Direction.BOTTOM_RIGHT);
        SetCliffsideLayers(cliffsideLeft, borderCornerBottomLeft, Direction.BOTTOM_LEFT);

        borderRight.SetActive(ShowEdge(Direction.RIGHT));
        borderLeft.SetActive(ShowEdge(Direction.LEFT));
        borderTopLeft.SetActive(ShowEdge(Direction.TOP_LEFT));
        borderTopRight.SetActive(ShowEdge(Direction.TOP_RIGHT));
        borderCornerTopLeft.SetActive(ShowEdge(Direction.LEFT) && (ShowEdge(Direction.TOP_LEFT) || ShowEdgeLower(Direction.TOP_LEFT)));
        borderCornerTopRight.SetActive(ShowEdge(Direction.RIGHT) && (ShowEdge(Direction.TOP_RIGHT) || ShowEdgeLower(Direction.TOP_RIGHT)));
        borderCornerBottomLeft.SetActive(ShowEdge(Direction.LEFT) && ShowEdge(Direction.BOTTOM_LEFT));
        borderCornerBottomRight.SetActive(ShowEdge(Direction.RIGHT) && ShowEdge(Direction.BOTTOM_RIGHT));

        borderCornerTopLeft.transform.Find("Pixel Fill").gameObject.SetActive(GetAdjacent(Direction.TOP_LEFT) == null || !ShowEdgeLower(Direction.TOP_LEFT));
        borderCornerTopRight.transform.Find("Pixel Fill").gameObject.SetActive(GetAdjacent(Direction.TOP_RIGHT) == null || !ShowEdgeLower(Direction.TOP_RIGHT));

        AdjustSideBorder(borderRight, Direction.TOP_RIGHT, Direction.RIGHT, Direction.BOTTOM_RIGHT);
        AdjustSideBorder(borderLeft, Direction.TOP_LEFT, Direction.LEFT, Direction.BOTTOM_LEFT);

        // Changes the height and positon of the side border based on 
        // the tile above it, below it, and next to it.
        void AdjustSideBorder(GameObject border, Direction topDir, Direction sideDir, Direction bottomDir){
            int extraTopLayers = 0;
            int diffBottomLayers = 0;
            if(GetAdjacent(topDir) != null && GetElevation(topDir) == tileState.GetElevation()) extraTopLayers = tileState.GetElevation() + 1;
            if(GetAdjacent(sideDir) != null && GetElevation(sideDir) < tileState.GetElevation() && extraTopLayers != 0) extraTopLayers -= GetElevation(sideDir) + 2;
            if(GetAdjacent(bottomDir) != null && GetElevation(bottomDir) > tileState.GetElevation()) diffBottomLayers = GetElevation(bottomDir) - tileState.GetElevation();
            border.transform.localScale = new Vector3(1, sideBorderSpriteInfo.GetScaleY() - ((extraTopLayers + diffBottomLayers) * sideBorderSpriteInfo.GetHeight()), 1);
            border.transform.localPosition = new Vector3(border.transform.localPosition.x, sideBorderSpriteInfo.GetInitY() 
                - (extraTopLayers * sideBorderSpriteInfo.GetHeight() * 0.5f) + (diffBottomLayers * sideBorderSpriteInfo.GetHeight() * 0.5f), border.transform.localPosition.z);
        }

        // Sets the height and position of a cliffside.
        void SetCliffsideLayers(GameObject cliffside, GameObject bottomCorner, Direction bottomDir){
            if(!cliffside.activeSelf) return;
            int layers;
            if(GetAdjacent(bottomDir) == null) layers = tileState.GetElevation() + 2;
            else layers = tileState.GetElevation() - GetElevation(bottomDir);

            foreach(Transform child in cliffside.transform){
                child.gameObject.SetActive(false);
            }
            cliffside.transform.GetChild(layers - 1).gameObject.SetActive(true);

            Transform cornerBorder = bottomCorner.transform;
            cornerBorder.localScale = new Vector3(1, cliffsideSpriteInfo.GetScaleY() * layers, 1);
            cornerBorder.localPosition = new Vector3(cornerBorder.localPosition.x, cliffsideSpriteInfo.GetInitY() 
                - (cliffsideSpriteInfo.GetHeight() * (layers - 1) * 0.5f), cornerBorder.localPosition.z); 
        }

        // Helper methods that get adjacent tiles and check them 
        // against certain conditions.
        Tile GetAdjacent(Direction dir){ return msc.GetTile(tileState.GetPosition() + new Vector2Int(dir.GetVector().x, dir.GetVector().y)); }
        int GetElevation(Direction dir){ return GetAdjacent(dir).GetTileState().GetElevation(); }
        bool ShowEdge(Direction dir){ return GetAdjacent(dir) == null || GetElevation(dir) < tileState.GetElevation(); }
        bool ShowEdgeLower(Direction dir){ return GetAdjacent(dir) == null || GetElevation(dir) > tileState.GetElevation(); }
    }

    // Gets the ID of the current tile.
    public virtual TileID GetID(){ 
        foreach(TileID id in Enum.GetValues(typeof(TileID))){
            if(id.GetPrefabName() == tileInfo.GetTileName()) return id;
        }
        return TileID.NULL_VALUE;
    }
}

// All possible tile IDs.
public enum TileID{
    GRASS_TILE,
    NULL_VALUE
}

// Used to find the correct prefab based on the tile ID.
public static class TileExtensions{
    public static String GetPrefabName(this TileID type){
        switch(type){
            case TileID.GRASS_TILE : return "Grass Tile";
        }
        return null;
    }
}

// Directions that one can go from a tile.
public enum Direction{
    TOP_RIGHT,
    TOP_LEFT,
    RIGHT,
    LEFT,
    BOTTOM_RIGHT,
    BOTTOM_LEFT,
    NULL_VALUE
}

// Converts a direction enum value to a direction vector.
public static class DirectionExtensions{
    public static Vector2Int GetVector(this Direction type){
        switch(type){
            case Direction.TOP_RIGHT : return new Vector2Int(1, -1);
            case Direction.TOP_LEFT : return new Vector2Int(0, -1);
            case Direction.RIGHT : return new Vector2Int(1, 0);
            case Direction.LEFT : return new Vector2Int(-1, 0);
            case Direction.BOTTOM_RIGHT : return new Vector2Int(0, 1);
            case Direction.BOTTOM_LEFT : return new Vector2Int(-1, 1);
        }
        return Vector2Int.zero;
    }

    public static Direction GetOpposite(this Direction type){
        switch(type){
            case Direction.TOP_RIGHT : return Direction.BOTTOM_LEFT;
            case Direction.TOP_LEFT : return Direction.BOTTOM_RIGHT;
            case Direction.RIGHT : return Direction.LEFT;
            case Direction.LEFT : return Direction.RIGHT;
            case Direction.BOTTOM_RIGHT : return Direction.TOP_LEFT;
            case Direction.BOTTOM_LEFT : return Direction.TOP_RIGHT;
        }
        return Direction.NULL_VALUE;
    }
}
