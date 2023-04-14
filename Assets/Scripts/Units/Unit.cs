using System;
using UnityEngine;

// Base class for all units.
public class Unit : MonoBehaviour{
    // Information about the unit.
    [SerializeField]
    protected UnitInfo unitInfo;
    protected UnitState unitState;

    // Controls unit animations.
    protected Animator unitAnimator;

    // The object that controls the current map, whether it's an editor or game.
    private MapStateController msc;

    // TODO: TEST FIELD, REMOVE IT!!!!
    public Action action;

    // Loads map state controller, as well as unit information.
    protected void Awake(){
        if(GameObject.Find("Editor Controller") != null) msc = GameObject.Find("Editor Controller").GetComponent<EditorController>().msc;
        else msc = GameObject.Find("Game Controller").GetComponent<GameController>().msc;

        unitState = ScriptableObject.CreateInstance<UnitState>();
        ResetUnitTokens();

        unitAnimator = GetComponent<Animator>();
    }

    // Resets the tokens a unit can currently use.
    public void ResetUnitTokens(){
        unitState.SetTraversalTokens(unitInfo.GetMaxTraversalTokens());
        unitState.SetActionTokens(unitInfo.GetMaxActionTokens());
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

    // Gets the ID of the current unit.
    public virtual UnitID GetID(){ 
        foreach(UnitID id in Enum.GetValues(typeof(UnitID))){
            if(id.GetPrefabName() == unitInfo.GetUnitName()) return id;
        }
        return UnitID.NULL_VALUE;
    }
}

// All possible unit IDs.
public enum UnitID{
    TST_UNIT,
    NULL_VALUE
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

// Information about what tokens an action expects.
public enum TokenType{
    ACTION_TOKEN,
    TRAVERSAL_TOKEN
}