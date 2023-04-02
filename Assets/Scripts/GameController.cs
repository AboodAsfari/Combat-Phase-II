using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour{
    [HideInInspector]
    public MapStateController gsc;
    
    void Start(){
        gsc = gameObject.AddComponent<MapStateController>();
        gsc.LoadFile();
    }
}
