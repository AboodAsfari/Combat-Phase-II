using System;
using System.Collections.Generic;
using UnityEngine;

// Controls the main game where players will fight.
public class GameController : MonoBehaviour{
    [HideInInspector]
    // The map state controller which is in charge
    // of any map-relation function such as saving, loading,
    // and interacting with map entities.
    public MapStateController msc;

    // Map drift related fields.
    [SerializeField]
    private float driftFactor = 0.1f;
    [SerializeField]
    private float driftFadeFactor = 0.5f;
    private Vector2 mapDrift = Vector2.zero;

    // Fields related to moving the map.
    private Vector2 mapOffset = Vector2Int.zero; 
    private Vector2 initMapOffset;
    private Vector2 initMousePos;
    private Vector2 prevMousePos;
    private bool isDragging = false;
    private float driftInterpolate = 0f;
    private bool camSnapOn = false;
    private bool camDriftOn = true;

    // Information about the tile sprite.
    private SpriteInfo spriteInfo;

    // The two players in the game.
    private PlayerController playerOne;
    private PlayerController playerTwo;

    // Tracks the currently selected entity.
    [HideInInspector]
    private Tile selectedTile;
    [HideInInspector]
    private Unit selectedUnit;

    // Tracks whether or not the game is in tile override.
    private bool inTileOverride = false;
    private HashSet<Vector2Int> overriddenTiles = new HashSet<Vector2Int>();
    private Action<Vector2Int> overrideFunction;
    
    // Creates a new map state controller and loads the tile sprite info.
    private void Start(){
        msc = gameObject.AddComponent<MapStateController>();
        msc.LoadFile();

        playerOne = gameObject.AddComponent<PlayerController>();
        playerTwo = gameObject.AddComponent<PlayerController>();
        playerOne.SetPlayerCol(PlayerColor.BLUE);
        playerTwo.SetPlayerCol(PlayerColor.RED);

        spriteInfo = Resources.Load("SpriteInfo/TileSpriteInfo") as SpriteInfo;

        // TODO: TEMP TESTING STUFF REMOVE THIS
        msc.SetUnit(new Vector2Int(5, 5), msc.CreateUnit(new Vector2Int(5, 5), UnitID.TST_UNIT, playerOne).GetComponent<Unit>());
        msc.SetUnit(new Vector2Int(7, 5), msc.CreateUnit(new Vector2Int(7, 5), UnitID.TST_UNIT, playerTwo).GetComponent<Unit>());
    }

    // Resets entity selection.
    public void ResetSelect(){
        selectedTile = null;
        selectedUnit = null;
    }

    // Selects an entity.
    public void SelectEntity(Tile tile){
        ResetSelect();
        selectedTile = tile;
        Debug.Log("Selected a tile at: " + tile.GetTileState().GetPosition());
    }
    public void SelectEntity(Unit unit){
        ResetSelect();
        selectedUnit = unit;
        Debug.Log("Selected a unit at: " + unit.GetUnitState().GetPosition());
    }

    // Enters tile override mode and highlights the given tiles.
    public void EnterTileOverride(HashSet<Tile> tiles, Action<Vector2Int> overrideFunction){
        inTileOverride = true;
        foreach(Tile t in tiles){
            overriddenTiles.Add(t.GetTileState().GetPosition());
            t.transform.Find("Tile Override").gameObject.SetActive(true);
        }
        this.overrideFunction = overrideFunction;
    }
    public void EnterTileOverride(HashSet<Vector2Int> tilePositions, Action<Vector2Int> overrideFunction){
        inTileOverride = true;
        foreach(Vector2Int pos in tilePositions){
            overriddenTiles.Add(pos);
            msc.GetTile(pos).transform.Find("Tile Override").gameObject.SetActive(true);
        }
        this.overrideFunction = overrideFunction;
    }

    // Exits tile override mode and removes override highlighting.
    public void ExitTileOverride(){
        inTileOverride = false;
        foreach(Vector2Int pos in overriddenTiles){
            msc.GetTile(pos).transform.Find("Tile Override").gameObject.SetActive(false);
        }
        overriddenTiles.Clear();
        overrideFunction = null;
    }

    // Getters.
    public bool IsInTileOverride(){ return inTileOverride; }
    public Action<Vector2Int> GetOverrideFunction(){ return overrideFunction; }

    // Allows map movement, and toggling camera options.
    private void Update(){
        // Moves the map and adds map drift when drag is over.
        if(Input.GetMouseButton(2)){
            if(!isDragging){
                isDragging = true;
                initMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                initMapOffset = mapOffset;
                mapDrift = Vector2.zero;
            }   
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseDiff = new Vector2(mousePos.x - initMousePos.x, mousePos.y - initMousePos.y);
            Vector2 posChange = camSnapOn ? Vector2Int.RoundToInt(Vector2.Scale(mouseDiff, new Vector2(1/spriteInfo.GetWidth(), 1/spriteInfo.GetHeight()))) : mouseDiff;
            prevMousePos = mousePos;
            mapOffset = initMapOffset + posChange;
            msc.SetMapPosition(mapOffset);
        }else if(isDragging){
            isDragging = false;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mapDrift = Vector2.Scale(new Vector2(mousePos.x - prevMousePos.x, mousePos.y - prevMousePos.y), new Vector2(driftFactor, driftFactor));
            driftInterpolate = 0f;
        }

        // Drifts the map.
        if(!mapDrift.Equals(Vector2.zero) && camDriftOn && !camSnapOn){
            mapOffset += mapDrift;
            msc.SetMapPosition(mapOffset);
            mapDrift = Vector2.Lerp(mapDrift, Vector2.zero, driftInterpolate);
            driftInterpolate += driftFadeFactor * Time.deltaTime;
        }

        // TODO: Remove these by adding user friendly functionality or putting them in a proper dev tool.
        // Toggles camera snap and camera drift, and executes actions.
        if(Input.GetKeyDown("c")){
            Debug.Log("Turning camera snap " + (camSnapOn ? "on" : "off"));
            camSnapOn = !camSnapOn;
        }else if(Input.GetKeyDown("d")){
            Debug.Log("Turning camera drift " + (camDriftOn ? "on" : "off"));
            camDriftOn = !camDriftOn;
        }else if(selectedUnit != null){
            if(Input.GetKeyDown("1")) selectedUnit.ExecuteAction(0);
            if(Input.GetKeyDown("2")) selectedUnit.ExecuteAction(1);
            if(Input.GetKeyDown("3")) selectedUnit.ExecuteAction(2);
            if(Input.GetKeyDown("4")) selectedUnit.ExecuteAction(3);
        }
    }
}
