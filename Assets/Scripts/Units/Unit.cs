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

    // References to either the editor controller or the game controller,
    // one of these will always be null.
    private EditorController editorController;
    private GameController gameController;

    // Array of actions a unit can take.
    [SerializeField]
    private Action[] actions = new Action[GlobalVars.UNIT_ACTION_COUNT];

    // Keeps track of how many uses an enhanced action has,
    // and what action to revert to when it is complete.
    private Tuple<Action, int> enhancedActionTracker;

    // Loads map state controller, as well as unit information.
    protected void Awake(){
        if(GameObject.Find("Editor Controller") != null){
            editorController = GameObject.Find("Editor Controller").GetComponent<EditorController>(); 
            msc = editorController.msc;
        }else{
            gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
            msc = gameController.msc;
        }

        unitState = ScriptableObject.CreateInstance<UnitState>();
        ResetUnitTokens();

        unitAnimator = GetComponent<Animator>();
    }

    // Executes an action.
    public void ExecuteAction(int actionNumber){
        if(actionNumber > GlobalVars.UNIT_ACTION_COUNT - 1 || actionNumber < 0) throw new ArgumentOutOfRangeException("There is no action associated with that number.");
        if(actions[actionNumber] == null) throw new InvalidOperationException("Unit does not have a full action list.");
        actions[actionNumber].ExecuteAction(GetComponent<Unit>(), gameController);
    }

    // Resets the tokens a unit can currently use.
    public void ResetUnitTokens(){
        unitState.SetTraversalTokens(unitInfo.GetMaxTraversalTokens());
        unitState.SetActionTokens(unitInfo.GetMaxActionTokens());
    }

    // Called when the unit owner is updated so that the correct 
    // animation layer can be used.
    public void UpdatedOwner(){
        bool isRed = GetUnitState().GetOwner().GetPlayerCol() == PlayerColor.RED;
        GetComponent<SpriteRenderer>().flipX = isRed;
        if(isRed) unitAnimator.SetLayerWeight(1, 1f);
        else unitAnimator.SetLayerWeight(1, 0f);
    }

    // Setters.
    public void SetHover(bool isHover){ 
        if(isHover) unitAnimator.Play("Hover", 0, unitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        else unitAnimator.Play("Idle", 0, unitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
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