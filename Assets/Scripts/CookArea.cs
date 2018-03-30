using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookArea : BuildArea
{
    public override void TryBuild()
    {
        CheckBuildables(DataManager.Instance.Treats);
    }
}