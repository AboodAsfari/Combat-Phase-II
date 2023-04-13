using System;
using UnityEngine;

// Base class for all units.
public class Unit : MonoBehaviour{
    // Information about the unit.
    [SerializeField]
    protected UnitInfo unitInfo;
    protected UnitState unitState;

    protected Animator unitAnimator;

    // The object that controls the current map, whether it's an editor or game.
    private MapStateController msc;

    // Loads map state controller, as well as unit information.
    protected void Awake(){
        if(GameObject.Find("Editor Controller") != null) msc = GameObject.Find("Editor Controller").GetComponent<EditorController>().msc;
        else msc = GameObject.Find("Game Controller").GetComponent<GameController>().msc;

        unitState = ScriptableObject.CreateInstance<UnitState>();

        unitAnimator = GetComponent<Animator>();
    }

    // Setters.
    public void SetHover(bool isHover){ 
        if(isHover) unitAnimator.Play("Hover", 0, unitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        else unitAnimator.Play("Idle", 0, unitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }

    public void UpdatedOwner(){
        bool isRed = GetUnitState().GetOwner().GetPlayerCol() == PlayerColor.RED;
        GetComponent<SpriteRenderer>().flipX = isRed;
        if(isRed) unitAnimator.SetLayerWeight(1, 1f);
        else unitAnimator.SetLayerWeight(1, 0f);
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
            case UnitID.TST_UNIT : return "Test Unit";
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