using System;
using System.Collections.Generic;
using UnityEngine;

// Controls the map editor.
public class EditorController : MonoBehaviour{
    [SerializeField]
    // The tile selector prefab.
    private GameObject tileSelector;

    [SerializeField]
    // The size of the tile selector grid.
    private Vector2 selectorGridSize;

    [HideInInspector]
    // The map state controller which is in charge
    // of any map-relation function such as saving, loading,
    // and interacting with map entities.
    public MapStateController msc;

    // Contains the tile selectors.
    private List<EditorTileSelector> selectorScripts;
    private GameObject tileSelectorContainer;

    // Fields related to moving the map.
    private Vector2Int mapOffset = Vector2Int.zero;
    private Vector2Int initMapOffset;
    private Vector2 initMousePos;
    private bool isDragging = false;
    
    // Information about the current tile that will be placed.
    private TileID currentTile = TileID.GRASS_TILE;
    private int tileElevation = 0;
    SpriteInfo spriteInfo;

    // Creates a map state controller, then creates the tile selectors.
    private void Start(){
        msc = gameObject.AddComponent<MapStateController>();

        tileSelectorContainer = new GameObject("Tile Selector Container");
        selectorScripts = new List<EditorTileSelector>();
        spriteInfo = Resources.Load("SpriteInfo/TileSpriteInfo") as SpriteInfo;
        CreateTileSelectors();
    }

    // Creats a new tile at the given position.
    public void CreateTile(Vector2Int pos){
        GameObject tile = msc.CreateTile(pos, currentTile, tileElevation);
        msc.SetTile(pos, tile.GetComponent<Tile>());
    }

    // Creates the grid of tile selectors.
    private void CreateTileSelectors(){
        tileSelectorContainer.transform.position = new Vector3(0f, 0f, 0f);

        for(int col = 0; col < selectorGridSize.x; col++){
            for(int row = 0; row < selectorGridSize.y; row++){  
                float xPosOffset = (spriteInfo.width * col) + (row % 2 == 0 ? SpriteInfo.TILE_HORIZONTAL_OFFSET : 0f);
                float yPosOffset = -(spriteInfo.height - SpriteInfo.TILE_VERTICAL_OFFSET) * row;
                Vector3 posOffset = new Vector3(xPosOffset, yPosOffset, 30);

                GameObject selector = Instantiate(tileSelector, msc.GetTopLeft() + posOffset, Quaternion.identity, tileSelectorContainer.transform);
                Vector3 oldPos = selector.transform.position;
                selector.transform.position = new Vector3(oldPos.x, oldPos.y, 0f);
                selector.GetComponent<EditorTileSelector>().SetPosition(new Vector2Int(col, row));
                selector.name = "Empty Tile Selector at: (" + col + ", " + row + ")";
                selectorScripts.Add(selector.GetComponent<EditorTileSelector>());
            }
        }
    }

    // Allows map movement and keybinds for dev
    private void Update(){
        // Moves the map (grid snap only).
        if(Input.GetMouseButton(2)){
            if(!isDragging){
                isDragging = true;
                initMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                initMapOffset = mapOffset;
            }   
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseDiff = new Vector2(mousePos.x - initMousePos.x, mousePos.y - initMousePos.y);
            Vector2Int posChange = Vector2Int.RoundToInt(Vector2.Scale(mouseDiff, new Vector2(1/spriteInfo.width, 1/spriteInfo.height)));
            mapOffset = initMapOffset + posChange;
            msc.SetMapPosition(mapOffset);
            tileSelectorContainer.transform.position = new Vector3(Math.Abs(mapOffset.y % 2) == 1 ? SpriteInfo.TILE_HORIZONTAL_OFFSET : 0f, 0f, 0f);
        }else isDragging = false;

        // TODO: Remove these by adding user friendly functionality or putting them in a proper dev tool.
        // Allows changing elevation, saving, and loading.
        if(Input.GetKeyDown("1")){
            Debug.Log("Setting elevation to: -1");
            tileElevation = -1;
        }else if(Input.GetKeyDown("2")){
            Debug.Log("Setting elevation to: 0");
            tileElevation = 0;
        }else if(Input.GetKeyDown("3")){
            Debug.Log("Setting elevation to: 1");
            tileElevation = 1;
        }else if(Input.GetKeyDown("s")){
            Debug.Log("Saving current edit...");
            msc.SaveFile();
        }else if(Input.GetKeyDown("l")){
            Debug.Log("Loading save file...");
            msc.LoadFile();
        }
    }

    // Getters.
    public Vector2Int GetMapOffset(){ return mapOffset; }
}
