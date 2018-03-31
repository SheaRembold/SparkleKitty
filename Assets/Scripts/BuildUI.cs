using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildUI : MonoBehaviour
{
    public void ShowBuild()
    {
        PlacementManager.Instance.SetArea(AreaType.Build);
        UIManager.Instance.ShowUI(gameObject);
    }

    public void HideBuild()
    {
        PlacementManager.Instance.SetArea(AreaType.Play);
        UIManager.Instance.GoBack();
    }
}