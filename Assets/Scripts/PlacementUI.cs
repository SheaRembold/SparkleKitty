using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementUI : MonoBehaviour
{
    [SerializeField]
    GameObject LookingHelp;
    [SerializeField]
    GameObject PlaceHelp;

    public void ShowLooking()
    {
        PlaceHelp.SetActive(false);
        LookingHelp.SetActive(true);
    }

    public void ShowPlace()
    {
        LookingHelp.SetActive(false);
        PlaceHelp.SetActive(true);
    }
}