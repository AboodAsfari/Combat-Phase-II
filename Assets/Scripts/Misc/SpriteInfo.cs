using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
// Static information about a tile.
public class SpriteInfo : ScriptableObject{
    public float width, height, initX, initY, scaleX, scaleY;

    // Tile offset information.
    public const float TILE_HORIZONTAL_OFFSET = 30f/32f;
    public const float TILE_VERTICAL_OFFSET = 15f/32f;
    public const float TILE_ELEVATION_OFFSET = 10f/32f;
}
