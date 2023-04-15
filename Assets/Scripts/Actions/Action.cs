using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : ScriptableObject{
    [SerializeField]
    protected String actionName;

    [SerializeField]
    [TextArea(2,10)]
    private String actionDescription;

    [SerializeField]
    private Action enhancesInto;

    [SerializeField]
    private TokenType tokenType;

    [SerializeField]
    private int tokenCost;

    [SerializeField]
    private bool isWhitelist;

    [SerializeField]
    private List<UnitID> unitList;

    [SerializeField]
    private int actionRange;

    public virtual String CanExecuteAction(Unit unit, GameController gc){
        int tokensAvailable = unit.GetUnitState().GetActionTokens();
        if(tokenType == TokenType.TRAVERSAL_TOKEN) tokensAvailable += unit.GetUnitState().GetTraversalTokens();
        if(tokensAvailable < tokenCost) return "Not enough tokens";
        return "";  
    }

    public virtual void ConsumeTokens(Unit unit, GameController gc){
        if(!CanExecuteAction(unit, gc).Equals("")) throw new InvalidOperationException("Unit does not have enough tokens.");
        if(tokenType != TokenType.TRAVERSAL_TOKEN) unit.GetUnitState().ConsumeActionTokens(tokenCost);
        else if(tokenCost <= unit.GetUnitState().GetTraversalTokens()) unit.GetUnitState().ConsumeTraversalTokens(tokenCost);
        else{
            int actionTokensConsumed = tokenCost - unit.GetUnitState().GetTraversalTokens();
            unit.GetUnitState().SetTraversalTokens(0);
            unit.GetUnitState().ConsumeActionTokens(actionTokensConsumed);
        }
    }

    public virtual void EnforceUnitList(Unit unit, GameController gc){
        if(isWhitelist){
            bool found = false;
            foreach(UnitID id in unitList){
                if(id == unit.GetID()){
                    found = true;
                    break;
                }
            }
            if(!found) throw new InvalidOperationException("Unit is not in the whitelist.");
        }else{
            foreach(UnitID id in unitList){
                if(id == unit.GetID()) throw new InvalidOperationException("Unit is in the blacklist.");
            }
        }
    }

    public void SetCurrentAction(Unit unit, GameController gc){
        ActionUtils.currentAction = this;
        ActionUtils.currentController = gc;
        ActionUtils.currentUnit = unit;
    }

    // Executes the action.
    public abstract void ExecuteAction(Unit unit, GameController gc);

    // Getters.
    public virtual String GetActionDescription(Unit unit, GameController gc){ return actionDescription; }
    public String GetActionName(){ return actionName; }
    public TokenType GetTokenType(){ return tokenType; }
    public int GetTokenCost(){ return tokenCost; }
    public bool GetIsWhitelist(){ return isWhitelist; }
    public List<UnitID> GetUnitList(){ return unitList; }
    public int GetActionRange(){ return actionRange; }
}