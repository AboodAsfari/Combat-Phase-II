using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestAction", menuName = "Actions/TestAction")]
public class TestAction : Action{
    public override void ExecuteAction(Unit unit, GameController gc){
        SetCurrentAction(unit, gc);
        EnforceUnitList(unit, gc);
        gc.EnterTileOverride(ActionUtils.GetNearbyUnits(unit.GetUnitState().GetPosition(), GetActionRange()), ActionUtils.MoveUnit);
    }
}