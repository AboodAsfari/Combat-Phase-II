using System;
using UnityEditor;
using UnityEngine;

// Base class for all units.
public abstract class Unit : MonoBehaviour{
    // Information about the unit.
    protected UnitInfo unitInfo;
    protected UnitState unitState;

    // The object that controls the current map, whether it's an editor or game.
    private MapStateController msc;

    // Loads map state controller, as well as unit information.
    protected void Awake(){
        if(GameObject.Find("Editor Controller") != null) msc = GameObject.Find("Editor Controller").GetComponent<EditorController>().msc;
        else msc = GameObject.Find("Game Controller").GetComponent<GameController>().msc;

        unitInfo = Resources.Load("UnitInfo/" + this.GetType().ToString() + "Info") as UnitInfo;
        unitState = ScriptableObject.CreateInstance<UnitState>();
    }

    // Getters.
    public UnitInfo GetUnitInfo(){ return unitInfo; }
    public UnitState GetUnitState(){ return unitState; }
}

// All possible unit IDs.
public enum UnitID{
    TST_UNIT
}

// Used to find the correct prefab based on the tile ID.
public static class UnitExtensions{
    public static String GetPrefabName(this UnitID type){
        switch(type){
            case UnitID.TST_UNIT : return "TestUnit";
        }
        return null;
    }
}

// A list of tags units can have.
public enum UnitTag{
    COMMANDER,
    OFFENSIVE,
    SUPPORT,
    BUILDER,
    OPERATOR
}