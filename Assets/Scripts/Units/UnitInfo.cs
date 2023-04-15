using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitInfo", menuName = "Entity Info/Unit Info")]
// Static information about a unit.
public class UnitInfo : ScriptableObject{
    // The name of the unit.
    [SerializeField]
    private String unitName;

    // The main type a unit falls under.
    [SerializeField]
    private UnitTag unitType;

    // A list of tags that this unit has.
    [SerializeField]
    private List<UnitTag> unitTags;

    // The maximum health a unit has by default.
    [SerializeField]
    private int maxHealth;

    // The maximum number of action tokens a unit can have by default.
    [SerializeField]
    private int maxTraversalTokens;
    [SerializeField]
    private int maxActionTokens;

    // Whether or not the unit can be walked through.
    [SerializeField]
    private bool canWalkThrough;

    // Getters.
    public String GetUnitName(){ return unitName; }
    public String GetUnitType(){ return unitType.ToString(); }
    public List<UnitTag> GetUnitTags(){ return unitTags; }
    public int GetMaxHealth(){ return maxHealth; }
    public int GetMaxTraversalTokens(){ return maxTraversalTokens; }
    public int GetMaxActionTokens(){ return maxActionTokens; }
    public bool GetCanWalkThrough(){ return canWalkThrough; }
}
