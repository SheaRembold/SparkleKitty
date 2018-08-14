using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementUI : MonoBehaviour
{
    [SerializeField]
    GameObject LookingHelp;
    [SerializeField]
    GameObject PlaceHelp;
    [SerializeField]
    GameObject LookingItemHelp;
    [SerializeField]
    GameObject PlaceItemHelp;

    public void ShowLooking()
    {
        LookingItemHelp.SetActive(false);
        PlaceItemHelp.SetActive(false);
        PlaceHelp.SetActive(false);
        LookingHelp.SetActive(true);
    }

    public void ShowPlace()
    {
        LookingItemHelp.SetActive(false);
        PlaceItemHelp.SetActive(false);
        LookingHelp.SetActive(false);
        PlaceHelp.SetActive(true);
    }

    public void ShowLookingItem()
    {
        LookingHelp.SetActive(false);
        PlaceHelp.SetActive(false);
        PlaceItemHelp.SetActive(false);
        LookingItemHelp.SetActive(true);
    }

    public void ShowPlaceItem()
    {
        LookingHelp.SetActive(false);
        PlaceHelp.SetActive(false);
        LookingItemHelp.SetActive(false);
        PlaceItemHelp.SetActive(true);
    }
}