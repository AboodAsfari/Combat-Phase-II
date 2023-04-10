using System;
using UnityEngine;

// Used to place tiles in the map editor.
public class EditorTileSelector : MonoBehaviour{
    // The grid position of the tile selector.
    private Vector2Int pos;
    
    // Prevents multiple tiles from being added to one position.
    private bool occupied = false;

    // The editor controller, used to get map offset and tile information.
    private EditorController editorController;

    // Gets the editor controller.
    private void Start(){
        editorController = GameObject.Find("Editor Controller").GetComponent<EditorController>();
    }

    // Places a tile if the selector is clicked.
    private void OnMouseOver(){
        Vector2Int posFix = (new Vector2Int((int) -Math.Ceiling(pos.y / 2f), 0) + Vector2Int.Scale(editorController.GetMapOffset(), new Vector2Int(-1, 1)))
            - new Vector2Int((int) Math.Floor(editorController.GetMapOffset().y / 2f), 0);
        if(!occupied && Input.GetMouseButton(0) && pos != null && editorController.msc.GetTile(pos + posFix) == null){
            occupied = true;
            editorController.CreateTile(pos + posFix);
        }
    }

    // Turns on the hover visual for a tile when the mouse is over it.
    private void OnMouseEnter(){
        transform.Find("Tile Hover").gameObject.SetActive(true);
    }

    // Turns off the hover visual for a tile when the mouse 
    // is no longer over it.
    private void OnMouseExit(){
        transform.Find("Tile Hover").gameObject.SetActive(false);
    }

    // Turns off the occupied flag once the tile is finished creating.
    private void Update(){
        Vector2Int posFix = (new Vector2Int((int) -Math.Ceiling(pos.y / 2f), 0) + Vector2Int.Scale(editorController.GetMapOffset(), new Vector2Int(-1, 1)))
            - new Vector2Int((int) Math.Floor(editorController.GetMapOffset().y / 2f), 0);
        if(occupied && editorController.msc.GetTile(pos + posFix) != null){
            occupied = false;
        }
    }
    
    // Setters.
    public void SetOccupied(bool occupied){ this.occupied = occupied; }
    public void SetPosition(Vector2Int pos){ this.pos = pos; }

    // Getters.
    public bool IsOccupied(){ return occupied; }
    public Vector2Int GetPosition(){ return pos; }
}
