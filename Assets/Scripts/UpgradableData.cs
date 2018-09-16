using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaterialType { Cardboard, Stone }

[CreateAssetMenu()]
public class UpgradableData : BuildableData
{
    public MaterialType MaterialType = MaterialType.Cardboard;
    public int Level;
    public PlacableData[] UpgradeRequirements;
    public UpgradableData Upgrade;
}
