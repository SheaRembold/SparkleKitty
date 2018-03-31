using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookUI : MonoBehaviour
{
    public void ShowCook()
    {
        PlacementManager.Instance.SetArea(AreaType.Cook);
        UIManager.Instance.ShowUI(gameObject);
    }

    public void HideCook()
    {
        PlacementManager.Instance.SetArea(AreaType.Play);
        UIManager.Instance.GoBack();
    }
}