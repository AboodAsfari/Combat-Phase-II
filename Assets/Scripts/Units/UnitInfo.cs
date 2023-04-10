using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
// Static information about a unit.
public class UnitInfo : ScriptableObject{
    // The name of the unit.
    public String unitName;

    // The main type a unit falls under.
    public UnitTag unitType;

    // A list of tags that this unit has.
    public List<UnitTag> unitTags;

    // The maximum health a unit has by default.
    public int maxHealth;

    // The maximum number of action tokens a unit can have by default.
    public int maxTraversalTokens;
    public int maxActionTokens;

    // Whether or not the unit can be walked through.
    public bool canWalkThrough;
}
