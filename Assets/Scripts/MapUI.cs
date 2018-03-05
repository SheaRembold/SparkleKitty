using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    public GameObject Map;

    private void OnEnable()
    {
        Map.SetActive(true);
        PlacementManager.Instance.SetArea(AreaType.None);
    }

    private void OnDisable()
    {
        Map.SetActive(false);
        PlacementManager.Instance.SetArea(AreaType.Play);
    }
}