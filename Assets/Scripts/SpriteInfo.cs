using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpriteInfo : ScriptableObject{
    public float width, height;

    public const float TILE_HORIZONTAL_OFFSET = 30f/32f;
    public const float TILE_VERTICAL_OFFSET = 15f/32f;
}
