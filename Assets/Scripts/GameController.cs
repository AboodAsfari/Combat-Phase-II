using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour{
    [HideInInspector]
    public MapStateController gsc;

    public float driftFactor = 0.1f;
    public float driftFadeFactor = 0.5f;
    private Vector2 mapOffset = Vector2Int.zero;
    private Vector2 mapDrift = Vector2.zero;
    private Vector2 initMapOffset;
    private Vector2 initMousePos;
    private Vector2 prevMousePos;
    private bool isDragging = false;
    private float driftInterpolate = 0f;
    private bool camSnapOn = false;
    private bool camDriftOn = true;

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
                mapDrift = Vector2.zero;
            }   
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseDiff = new Vector2(mousePos.x - initMousePos.x, mousePos.y - initMousePos.y);
            Vector2 posChange = camSnapOn ? Vector2Int.RoundToInt(Vector2.Scale(mouseDiff, new Vector2(1/spriteInfo.width, 1/spriteInfo.height))) : mouseDiff;
            prevMousePos = mousePos;
            mapOffset = initMapOffset + posChange;
            gsc.SetMapPosition(mapOffset);
        }else if(isDragging){
            isDragging = false;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mapDrift = Vector2.Scale(new Vector2(mousePos.x - prevMousePos.x, mousePos.y - prevMousePos.y), new Vector2(driftFactor, driftFactor));
            driftInterpolate = 0f;
        }

        if(!mapDrift.Equals(Vector2.zero) && camDriftOn && !camSnapOn){
            mapOffset += mapDrift;
            gsc.SetMapPosition(mapOffset);
            float newX = Mathf.Lerp(mapDrift.x, 0, driftInterpolate);
            float newY = Mathf.Lerp(mapDrift.y, 0, driftInterpolate);
            mapDrift = new Vector2(newX, newY);
            driftInterpolate += driftFadeFactor * Time.deltaTime;
        }

        if(Input.GetKeyDown("c")){
            Debug.Log("Turning camera snap " + (camSnapOn ? "on" : "off"));
            camSnapOn = !camSnapOn;
        }else if(Input.GetKeyDown("d")){
            Debug.Log("Turning camera drift " + (camDriftOn ? "on" : "off"));
            camDriftOn = !camDriftOn;
        }
    }
}
