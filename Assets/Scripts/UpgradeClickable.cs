using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UpgradeClickable : Clickable
{
    public override void Click()
    {
        UIManager.Instance.ShowUpgradeUI(this);
    }
}