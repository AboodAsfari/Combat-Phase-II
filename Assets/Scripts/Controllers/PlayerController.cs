using System;
using System.Collections.Generic;
using UnityEngine;

// Controls player game actions and information.
public class PlayerController : MonoBehaviour{
    private PlayerColor playerCol;

    public void SetPlayerCol(PlayerColor playerCol){ this.playerCol = playerCol; }

    public PlayerColor GetPlayerCol(){ return playerCol; }
}

public enum PlayerColor{
    BLUE,
    RED
}