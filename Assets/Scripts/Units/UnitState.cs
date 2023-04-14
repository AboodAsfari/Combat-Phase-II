using System;
using UnityEngine;

// Dynamic information about a unit.
public class UnitState : ScriptableObject{
    // The unit position in the game grid.
    private Vector2Int pos;

    // The player that owns this unit.
    private PlayerController owner;

    // Tracks the current tokens a unit can use.
    private int traversalTokens;
    private int actionTokens;

    // Setters.
    public void SetPosition(Vector2Int pos){ this.pos = pos; }
    public void SetOwner(PlayerController owner){ this.owner = owner; }
    public void SetTraversalTokens(int tokens){ this.traversalTokens = tokens; }
    public void SetActionTokens(int tokens){ this.actionTokens = tokens; }
    public void ConsumeTraversalTokens(int tokens){ this.traversalTokens -= tokens; }
    public void ConsumeActionTokens(int tokens){ this.actionTokens -= tokens; }

    // Getters.
    public Vector2Int GetPosition(){ return pos; }
    public PlayerController GetOwner(){ return owner; }
    public int GetTraversalTokens(){ return traversalTokens; }
    public int GetActionTokens(){ return actionTokens; }
}