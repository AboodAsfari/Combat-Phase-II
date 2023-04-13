using System;
using System.Collections.Generic;
using UnityEngine;

// Controls player game actions and information.
public class PlayerController : MonoBehaviour{
    private int playerNum;

    public void SetPlayerNum(int playerNum){ this.playerNum = playerNum; }

    public int GetPlayerNum(){ return playerNum; }
}
