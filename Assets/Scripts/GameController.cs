using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour{
    [HideInInspector]
    public MapStateController gsc;

    private Vector2 mapOffset = Vector2Int.zero;
    private Vector2 initMapOffset;
    private Vector2 initMousePos;
    private bool isDragging = false;
    bool isSnap = false;

    SpriteInfo spriteInfo;
    
    void Start(){
        gsc = gameObject.AddComponent<MapStateController>();
        gsc.LoadFile();

        spriteInfo = Resources.Load("SpriteInfo/TileSpriteInfo") as SpriteInfo;
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
            Vector2 posChange = isSnap ? Vector2Int.RoundToInt(Vector2.Scale(mouseDiff, new Vector2(1/spriteInfo.width, 1/spriteInfo.height))) : mouseDiff;
            mapOffset = initMapOffset + posChange;
            gsc.SetMapPosition(mapOffset);
        }else isDragging = false;

        if(Input.GetKeyDown("c")){
            Debug.Log("Set camera snap to: " + !isSnap);
            isSnap = !isSnap;
        }
    }
}
