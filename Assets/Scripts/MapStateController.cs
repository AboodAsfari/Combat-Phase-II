using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class MapStateController : MonoBehaviour{
    
    public Dictionary<Vector2Int, Tile> tileDict = new Dictionary<Vector2Int, Tile>();
    private Vector3 topLeft;
    SpriteInfo spriteInfo;
    
    private GameObject mapContainer;
    private GameObject tileContainer;
    
    private string destination;
	private BinaryFormatter bf = new BinaryFormatter();

    public void Awake(){
        topLeft = Vector3.Scale(Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, Camera.main.nearClipPlane)), new Vector3(1, 1, 0));
        spriteInfo = Resources.Load("SpriteInfo/TileSpriteInfo") as SpriteInfo;

        mapContainer = new GameObject("Map Container");
        tileContainer = new GameObject("Tile Container");
        tileContainer.transform.parent = mapContainer.transform;
        tileContainer.transform.position = topLeft;

        destination = Application.persistentDataPath + "/examplemap.dat";
    }

    public GameObject CreateTile(Vector2Int pos, TileID tileID, int elevation = 0){
        GameObject prefab = Resources.Load("Tiles/" + tileID.GetPrefabName()) as GameObject;
        GameObject tile = Instantiate(prefab, topLeft, Quaternion.identity, tileContainer.transform);
        Tile tileScript = tile.GetComponent<Tile>();
        if(GameObject.Find("Editor Controller") != null) tile.GetComponent<Tile>().SetEditing(true);
        tileScript.GetTileState().SetPosition(pos);
        tileScript.GetTileState().SetElevation(elevation);
        tile.name = tileScript.GetTileInfo().tileName + " at: (" + tileScript.GetTileState().GetPosition().x + ", " + tileScript.GetTileState().GetPosition().y + ")";
        tile.transform.localPosition = GetScreenPos(pos) + new Vector3(SpriteInfo.TILE_HORIZONTAL_OFFSET, 0, -31);
        
        return tile;
    }

    public void DeleteTile(Vector2Int pos, bool deleteDict = true){
        GameObject obj = tileDict[pos].gameObject;
        if(deleteDict) tileDict.Remove(pos);
        Destroy(obj);
    }

    public void ResetMap(){
        foreach(Vector2Int pos in tileDict.Keys){
            DeleteTile(pos, false);
        }
        tileDict.Clear();
    }

    public void SaveFile(){
        FileStream file;
        if(File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        GameData data = new GameData(tileDict);
        bf.Serialize(file, data);
        file.Close();
    }

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

    public void SetMapPosition(Vector2Int point){
        Vector2 pos = Vector2.Scale(point , new Vector2(spriteInfo.width, spriteInfo.height - SpriteInfo.TILE_VERTICAL_OFFSET));
        mapContainer.transform.position = pos;
    }

    public void SetTile(Vector2Int pos, Tile tile){ tileDict.Add(pos, tile); }

    public void SetTile(int x, int y, Tile tile){ SetTile(new Vector2Int(x, y), tile); }

    public Tile GetTile(Vector2Int pos){
        if(tileDict.ContainsKey(pos)) return tileDict[pos];
        return null;
    }

    public Tile GetTile(int x, int y){ return GetTile(new Vector2Int(x, y)); }

    public Vector3 GetTopLeft(){ return topLeft; }

    private Vector3 GetScreenPos(Vector2Int pos){
        SpriteInfo spriteInfo = Resources.Load("SpriteInfo/TileSpriteInfo") as SpriteInfo;
        float xPosOffset = (spriteInfo.width * pos.x) + (pos.y * SpriteInfo.TILE_HORIZONTAL_OFFSET);
        float yPosOffset = -(spriteInfo.height - SpriteInfo.TILE_VERTICAL_OFFSET) * pos.y;
        return new Vector3(xPosOffset, yPosOffset, 30);
    }
}

[System.Serializable]
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