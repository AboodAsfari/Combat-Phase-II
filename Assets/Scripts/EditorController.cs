using System;
using System.Collections.Generic;
using UnityEngine;

public class EditorController : MonoBehaviour{
    [SerializeField]
    private GameObject gridSelect;
    [SerializeField]
    private Vector2 selectorGridSize;

    [HideInInspector]
    public MapStateController gsc;

    private List<EmptyTileSelector> selectorScripts;
    private GameObject tileSelectorContainer;

    private Vector2Int mapOffset = Vector2Int.zero;
    private Vector2Int initMapOffset;
    private Vector2 initMousePos;
    private bool isDragging = false;
    
    private TileID currentTile = TileID.GRASS_TILE;
    private int tileElevation = 0;
    SpriteInfo spriteInfo;

    private void Start(){
        tileSelectorContainer = new GameObject("Tile Selector Container");
        gsc = gameObject.AddComponent<MapStateController>();
        selectorScripts = new List<EmptyTileSelector>();
        spriteInfo = Resources.Load("SpriteInfo/TileSpriteInfo") as SpriteInfo;
        CreateTileSelectors();
    }

    public void CreateTile(Vector2Int pos){
        GameObject tile = gsc.CreateTile(pos, currentTile, tileElevation);
        gsc.SetTile(pos, tile.GetComponent<Tile>());
    }

    private void CreateTileSelectors(){
        tileSelectorContainer.transform.position = new Vector3(0f, 0f, 0f);

        for(int col = 0; col < selectorGridSize.x; col++){
            for(int row = 0; row < selectorGridSize.y; row++){  
                float xPosOffset = (spriteInfo.width * col) + (row % 2 == 0 ? SpriteInfo.TILE_HORIZONTAL_OFFSET : 0f);
                float yPosOffset = -(spriteInfo.height - SpriteInfo.TILE_VERTICAL_OFFSET) * row;
                Vector3 posOffset = new Vector3(xPosOffset, yPosOffset, 30);

                GameObject selector = Instantiate(gridSelect, gsc.GetTopLeft() + posOffset, Quaternion.identity, tileSelectorContainer.transform);
                Vector3 oldPos = selector.transform.position;
                selector.transform.position = new Vector3(oldPos.x, oldPos.y, 0f);
                selector.GetComponent<EmptyTileSelector>().SetPosition(new Vector2Int(col, row));
                selector.name = "Empty Tile Selector at: (" + col + ", " + row + ")";
                selectorScripts.Add(selector.GetComponent<EmptyTileSelector>());
            }
        }
    }

    private void Update(){
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
            gsc.SetMapPosition(mapOffset);
            tileSelectorContainer.transform.position = new Vector3(Math.Abs(mapOffset.y % 2) == 1 ? SpriteInfo.TILE_HORIZONTAL_OFFSET : 0f, 0f, 0f);
        }else isDragging = false;

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
            gsc.SaveFile();
        }else if(Input.GetKeyDown("l")){
            Debug.Log("Loading save file...");
            gsc.LoadFile();
        }
    }

    public Vector2Int GetMapOffset(){ return mapOffset; }
}
