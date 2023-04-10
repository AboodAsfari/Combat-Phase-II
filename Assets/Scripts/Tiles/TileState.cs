using System;
using UnityEngine;

// Dynamic information about a tile.
public class TileState : ScriptableObject{
    // The tile position in the game grid.
    private Vector2Int pos;

    // The elevation of the tile.
    private int elevation = 0;

    // Parses the tile state into a serializable token.
    public static TileState ParseToken(String token){
        TileState state = ScriptableObject.CreateInstance<TileState>();
        Vector2Int extractedPos = Vector2Int.zero;

        String[] tokens = token.Split('&');
        foreach(String itemPair in tokens){
            String[] items = itemPair.Split('=');
            if(items[0] == "posx") extractedPos.x = int.Parse(items[1]);
            else if(items[0] == "posy") extractedPos.y = int.Parse(items[1]);
            else if(items[0] == "elevation") state.SetElevation(int.Parse(items[1]));
        }
        state.SetPosition(extractedPos);

        return state;
    }

    // Setters.
    public void SetPosition(Vector2Int pos){ this.pos = pos; }
    public void SetElevation(int elevation){ this.elevation = elevation; }

    // Getters.
    public Vector2Int GetPosition(){ return pos; }
    public int GetElevation(){ return elevation; }
    public String GetToken(){ return "posx=" + pos.x + "&posy=" + pos.y + "&elevation=" + elevation; }
}
