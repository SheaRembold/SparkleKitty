using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildUI : MonoBehaviour
{
    private void OnEnable()
    {
        PlacementManager.Instance.SetArea(AreaType.Build);
    }

    private void OnDisable()
    {
        PlacementManager.Instance.SetArea(AreaType.Play);
    }
}