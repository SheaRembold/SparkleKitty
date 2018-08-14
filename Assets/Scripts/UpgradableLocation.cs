using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradableLocation : PlacementLocation
{
    public override void Click(RaycastHit hit)
    {
        //UIManager.Instance.GetBuildUI(buildUI).ShowUpgrade(this, CurrentPlacable.Data as UpgradableData);
    }
}