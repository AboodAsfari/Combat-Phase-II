using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action")]
public abstract class Action : ScriptableObject{
    [SerializeField]
    private string actionName;

    [SerializeField]
    private TokenType tokenType;

    [SerializeField]
    private int tokenCost;

    [SerializeField]
    private bool isWhitelist;

    [SerializeField]
    private List<UnitID> unitList;

    public virtual String CanExecuteAction(Unit unit, MapStateController msc){
        return "TEMP"; // TODO: Add unit tokens so I can add a proper condition here.
    }   

    // Executes the action.
    public abstract void ExecuteAction(Unit unit, MapStateController msc);
    public abstract void EnhancedExecuteAction(Unit unit, MapStateController msc);

    // Getters.
    public abstract String GetActionDescription(Unit unit, MapStateController msc);
    public abstract String GetEnhancedActionDescription(Unit unit, MapStateController msc);

    // TODO: Move this later to where the units have their tokens.
    private enum TokenType{
        ACTION_TOKEN,
        TRAVERSAL_TOKEN
    }
}