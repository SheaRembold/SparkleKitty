using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    public GameObject Map;

    private void Awake()
    {
        if (!GoogleARCore.AndroidPermissionsManager.IsPermissionGranted("android.permission.ACCESS_FINE_LOCATION"))
            GoogleARCore.AndroidPermissionsManager.RequestPermission("android.permission.ACCESS_FINE_LOCATION");
    }

    private void OnEnable()
    {
        Map.SetActive(true);
        PlacementManager.Instance.SetArea(AreaType.None);
    }

    private void OnDisable()
    {
        if (Map != null)
        {
            Map.SetActive(false);
            PlacementManager.Instance.ResetLastArea();
        }
    }
}