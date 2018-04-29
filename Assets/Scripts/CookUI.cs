using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookUI : BuildUI
{
    public override void Show(PlacementLocation placementLocation)
    {
        PlacementManager.Instance.SetArea(AreaType.Cook);
        PlacementManager.Instance.GetCookArea().SetPlacementLocation(placementLocation);
        UIManager.Instance.ShowUI(gameObject);
    }

    public override void Hide()
    {
        PlacementManager.Instance.SetArea(AreaType.Play);
        UIManager.Instance.GoBack();
    }
}