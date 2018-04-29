using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeWorkUI : BuildWorkUI
{
    protected override void HandleInput()
    {
        if (currentPos.x <= line.position.x + tolerance && currentPos.x >= line.position.x - tolerance
            && lastPos.x <= line.position.x + tolerance && lastPos.x >= line.position.x - tolerance)
        {
            amount += Mathf.Abs(currentPos.y - lastPos.y);
        }
    }
}