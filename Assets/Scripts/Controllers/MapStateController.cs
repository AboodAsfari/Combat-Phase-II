using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// The main game class which will control the map state. 
// This means every entity will be accessed through here.
public class MapStateController : MonoBehaviour{    
    // The tile map.
    public Dictionary<Vector2Int, Tile> tileDict = new Dictionary<Vector2Int, Tile>();
    
    // Information used to place tiles in the correct world position.
    private Vector3 topLeft;
    SpriteInfo spriteInfo;

    // Stores map entities: tiles.
    private GameObject mapContainer;
    private GameObject tileContainer;
    
    // Fields related to loading map info.
    private string destination;
	private BinaryFormatter bf = new BinaryFormatter();

    // Loads position related info (top left position and tile sprite info), 
    // initializes map containers, and declares save file location.
    public void Awake(){
        topLeft = Vector3.Scale(Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, Camera.main.nearClipPlane)), new Vector3(1, 1, 0));
        spriteInfo = Resources.Load("SpriteInfo/TileSpriteInfo") as SpriteInfo;

        mapContainer = new GameObject("Map Container");
        tileContainer = new GameObject("Tile Container");
        tileContainer.transform.parent = mapContainer.transform;
        tileContainer.transform.position = topLeft;

        destination = Application.persistentDataPath + "/examplemap.dat";
    }

    // Creates and returns a new tile using a position, ID, and tile elevation.
    public GameObject CreateTile(Vector2Int pos, TileID tileID, int elevation = 0){
        // Creates the tile object (in editing mode, if the editor is open).
        GameObject prefab = Resources.Load("Tiles/" + tileID.GetPrefabName()) as GameObject;
        GameObject tile = Instantiate(prefab, topLeft, Quaternion.identity, tileContainer.transform);
        Tile tileScript = tile.GetComponent<Tile>();
        if(GameObject.Find("Editor Controller") != null) tile.GetComponent<Tile>().SetEditing(true);

        // Sets tile properties and position.
        tileScript.GetTileState().SetPosition(pos);
        tileScript.GetTileState().SetElevation(elevation);
        tile.name = tileScript.GetTileInfo().tileName + " at: (" + tileScript.GetTileState().GetPosition().x + ", " + tileScript.GetTileState().GetPosition().y + ")";
        tile.transform.localPosition = GetScreenPos(pos) + new Vector3(SpriteInfo.TILE_HORIZONTAL_OFFSET, 0, -31) 
            + new Vector3(0, SpriteInfo.TILE_ELEVATION_OFFSET * elevation, 0);

        // Sets the sorting order of the tile and its children.
        tile.GetComponent<SpriteRenderer>().sortingOrder = elevation;
        tile.transform.Find("Tile Hover").GetComponent<SpriteRenderer>().sortingOrder = elevation;
        foreach(Transform child in tile.transform.Find("Borders & Cliffside")){
            if(child.childCount > 0){
                foreach(Transform cliffsideType in child.transform){
                    if(cliffsideType.GetComponent<SpriteRenderer>().sortingOrder == 0) cliffsideType.GetComponent<SpriteRenderer>().sortingOrder = elevation;
                    foreach(Transform cliffsideElement in cliffsideType.transform){
                        if(cliffsideElement.GetComponent<SpriteRenderer>().sortingOrder == 0) cliffsideElement.GetComponent<SpriteRenderer>().sortingOrder = elevation;
                    }
                }
            } else if(child.GetComponent<SpriteRenderer>().sortingOrder == 0) child.GetComponent<SpriteRenderer>().sortingOrder = elevation;
        }
        
        return tile;
    }

    // Removes a tile from the tile map.
    public void DeleteTile(Vector2Int pos, bool deleteDict = true){
        GameObject obj = tileDict[pos].gameObject;
        if(deleteDict) tileDict.Remove(pos);
        Destroy(obj);
        foreach(Tile t in tileDict.Values){
            t.UpdateVisual();
        }
    }

    // Resets the tile map.
    public void ResetMap(){
        foreach(Vector2Int pos in tileDict.Keys){
            DeleteTile(pos, false);
        }
        tileDict.Clear();
    }

    // Saves the current tile map.
    public void SaveFile(){
        FileStream file;
        if(File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        GameData data = new GameData(tileDict);
        bf.Serialize(file, data);
        file.Close();
    }

    // Loads a map from a save file.
    public void LoadFile(){
        FileStream file;
        if(File.Exists(destination)) file = File.OpenRead(destination);
		else{
            Debug.LogError("File not found");
            return;
        }
 
        GameData data = (GameData) bf.Deserialize(file);
        file.Close();

        ResetMap();
        foreach(Tuple<int, int> tuplePos in data.savedTiledDict.Keys){
            Vector2Int pos = new Vector2Int(tuplePos.Item1, tuplePos.Item2);
            Tuple<TileID, String> tileTuple = data.savedTiledDict[tuplePos];
            TileState tileState = TileState.ParseToken(tileTuple.Item2);
            GameObject tile = CreateTile(pos, tileTuple.Item1, tileState.GetElevation());
            SetTile(pos, tile.GetComponent<Tile>());
        }
    }

    // Setters.
    public void SetMapPosition(Vector2 point){
        Vector2 pos = Vector2.Scale(point , new Vector2(spriteInfo.width, spriteInfo.height - SpriteInfo.TILE_VERTICAL_OFFSET));
        mapContainer.transform.position = pos;
    }

    public void SetTile(Vector2Int pos, Tile tile){ 
        tileDict.Add(pos, tile); 
        foreach(Tile t in tileDict.Values){
            t.UpdateVisual();
        }
    }
    public void SetTile(int x, int y, Tile tile){ SetTile(new Vector2Int(x, y), tile); }

    // Getters.
    public Tile GetTile(Vector2Int pos){
        if(tileDict.ContainsKey(pos)) return tileDict[pos];
        return null;
    }
    public Tile GetTile(int x, int y){ return GetTile(new Vector2Int(x, y)); }

    private Vector3 GetScreenPos(Vector2Int pos){
        SpriteInfo spriteInfo = Resources.Load("SpriteInfo/TileSpriteInfo") as SpriteInfo;
        float xPosOffset = (spriteInfo.width * pos.x) + (pos.y * SpriteInfo.TILE_HORIZONTAL_OFFSET);
        float yPosOffset = -(spriteInfo.height - SpriteInfo.TILE_VERTICAL_OFFSET) * pos.y;
        return new Vector3(xPosOffset, yPosOffset, 30);
    }

    public Vector3 GetTopLeft(){ return topLeft; }
}

[System.Serializable]
// Takes current map data and stores it in a serializable object.
public class GameData{
	public Dictionary<Tuple<int, int>, Tuple<TileID, String>> savedTiledDict;

	public GameData(Dictionary<Vector2Int, Tile> tileDict){
		savedTiledDict = new Dictionary<Tuple<int, int>, Tuple<TileID, String>>();
        foreach(Vector2Int pos in tileDict.Keys){
            Tuple<int, int> nonVectorPos = new Tuple<int, int>(pos.x, pos.y);
            Tuple<TileID, String> tileTuple = new Tuple<TileID, String>(tileDict[pos].GetID(), tileDict[pos].GetTileState().GetToken());
            savedTiledDict.Add(nonVectorPos, tileTuple);
        }
	}
 }