using UnityEngine;

[CreateAssetMenu(fileName = "TestAction", menuName = "Actions/TestAction")]
public class TestAction : Action{
    public override void ExecuteAction(Unit unit, MapStateController msc){
        EnforceUnitList(unit, msc);
        ConsumeTokens(unit, msc);
        throw new System.NotImplementedException();
    }
}