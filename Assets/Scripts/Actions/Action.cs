using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : ScriptableObject{
    [SerializeField]
    private String actionName;

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

    public virtual String CanExecuteAction(Unit unit, MapStateController msc){
        int tokensAvailable = unit.GetUnitState().GetActionTokens();
        if(tokenType == TokenType.TRAVERSAL_TOKEN) tokensAvailable += unit.GetUnitState().GetTraversalTokens();
        if(tokensAvailable < tokenCost) return "Not enough tokens";
        return "";
    }   

    public virtual void ConsumeTokens(Unit unit, MapStateController msc){
        if(!CanExecuteAction(unit, msc).Equals("")) throw new InvalidOperationException("Unit does not have enough tokens.");
        if(tokenType != TokenType.TRAVERSAL_TOKEN) unit.GetUnitState().ConsumeActionTokens(tokenCost);
        else if(tokenCost <= unit.GetUnitState().GetTraversalTokens()) unit.GetUnitState().ConsumeTraversalTokens(tokenCost);
        else{
            int actionTokensConsumed = tokenCost - unit.GetUnitState().GetTraversalTokens();
            unit.GetUnitState().SetTraversalTokens(0);
            unit.GetUnitState().ConsumeActionTokens(actionTokensConsumed);
        }
    }

    public virtual void EnforceUnitList(Unit unit, MapStateController msc){
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

    // Executes the action.
    public abstract void ExecuteAction(Unit unit, MapStateController msc);

    // Getters.
    public virtual String GetActionDescription(Unit unit, MapStateController msc){ return actionDescription; }
    public String GetActionName(){ return actionName; }
    public TokenType GetTokenType(){ return tokenType; }
    public int GetTokenCost(){ return tokenCost; }
    public bool GetIsWhitelist(){ return isWhitelist; }
    public List<UnitID> GetUnitList(){ return unitList; }
}