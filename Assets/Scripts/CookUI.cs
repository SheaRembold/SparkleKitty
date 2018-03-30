using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookUI : MonoBehaviour
{
    private void OnEnable()
    {
        PlacementManager.Instance.SetArea(AreaType.Cook);
    }

    private void OnDisable()
    {
        PlacementManager.Instance.SetArea(AreaType.Play);
    }
}