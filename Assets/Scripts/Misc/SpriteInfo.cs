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

    // Tile offset information.
    public const float TILE_HORIZONTAL_OFFSET = 30f/32f;
    public const float TILE_VERTICAL_OFFSET = 15f/32f;
    public const float TILE_ELEVATION_OFFSET = 10f/32f;
}
