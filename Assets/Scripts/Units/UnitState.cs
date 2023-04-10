using System;
using UnityEngine;

// Dynamic information about a unit.
public class UnitState : ScriptableObject{
    // The unit position in the game grid.
    private Vector2Int pos;

    // The player that owns this unit.
    private PlayerController owner;

    // Setters.
    public void SetPosition(Vector2Int pos){ this.pos = pos; }
    public void SetOwner(PlayerController owner){ this.owner = owner; }

    // Getters.
    public Vector2Int GetPosition(){ return pos; }
    public PlayerController GetOwner(){ return owner; }
}