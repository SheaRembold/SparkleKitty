using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CatData : PlacableData
{
    public MaterialType RequiredTowerType;
    public int RequiredTowerLevel;
    public PlacableData[] OtherRequirements;
}
