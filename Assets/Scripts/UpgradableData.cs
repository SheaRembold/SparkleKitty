using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaterialType { None, Cardboard, Felt, ShaggyCarpet }

[CreateAssetMenu()]
public class UpgradableData : BuildableData
{
    public MaterialType MaterialType = MaterialType.None;
    public int Level;
    public PlacableData[] UpgradeRequirements;
    public UpgradableData Upgrade;
}
