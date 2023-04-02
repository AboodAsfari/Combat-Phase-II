using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyTileSelector : MonoBehaviour{
    private Vector2Int pos;
    private bool occupied = false;
    private EditorController editorController;

    private void Start(){
        editorController = GameObject.Find("Editor Controller").GetComponent<EditorController>();
    }

    private void OnMouseOver(){
        Vector2Int posFix = (new Vector2Int((int) -Math.Ceiling(pos.y / 2f), 0) + Vector2Int.Scale(editorController.GetMapOffset(), new Vector2Int(-1, 1)))
            - new Vector2Int((int) Math.Floor(editorController.GetMapOffset().y / 2f), 0);
        if(!occupied && Input.GetMouseButton(0) && pos != null && editorController.gsc.GetTile(pos + posFix) == null){
            occupied = true;
            editorController.CreateTile(pos + posFix);
        }
    }

    private void Update(){
        Vector2Int posFix = (new Vector2Int((int) -Math.Ceiling(pos.y / 2f), 0) + Vector2Int.Scale(editorController.GetMapOffset(), new Vector2Int(-1, 1)))
            - new Vector2Int((int) Math.Floor(editorController.GetMapOffset().y / 2f), 0);
        if(occupied && editorController.gsc.GetTile(pos + posFix) != null){
            occupied = false;
        }
    }
    
    public void SetOccupied(bool occupied){ this.occupied = occupied; }

    public void SetPosition(Vector2Int pos){ this.pos = pos; }

    public bool IsOccupied(){ return occupied; }

    public Vector2Int GetPosition(){ return pos; }
}
