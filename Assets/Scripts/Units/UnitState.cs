using System;
using UnityEngine;

// Dynamic information about a unit.
public class UnitState : ScriptableObject{
    // The unit position in the game grid.
    private Vector2Int pos;

    // Setters.
    public void SetPosition(Vector2Int pos){ this.pos = pos; }

    // Getters.
    public Vector2Int GetPosition(){ return pos; }
}