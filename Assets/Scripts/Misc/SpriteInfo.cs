using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteInfo")]
// Static information about a tile.
public class SpriteInfo : ScriptableObject{
    [SerializeField]
    private float width, height, initX, initY, scaleX, scaleY;

    // Getters
    public float GetWidth(){ return width; }
    public float GetHeight(){ return height; }
    public float GetInitX(){ return initX; }
    public float GetInitY(){ return initY; }
    public float GetScaleX(){ return scaleX; }
    public float GetScaleY(){ return scaleY; }
}
