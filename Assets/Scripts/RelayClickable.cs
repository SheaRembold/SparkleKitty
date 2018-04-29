using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelayClickable : Clickable
{
    [System.NonSerialized]
    public Clickable ParentClickable;

    public override void Click()
    {
        ParentClickable.Click();
    }
}