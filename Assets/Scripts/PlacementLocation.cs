using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlacementLocation : Clickable
{
    public PlacableData StartingPlacable;
    [System.NonSerialized]
    public Placable CurrentPlacable;
    [SerializeField]
    protected string buildUI;

    public void SetPlacable(PlacableData placable)
    {
        if (CurrentPlacable != null)
        {
            PlacementManager.Instance.Remove(CurrentPlacable);
            CurrentPlacable = null;
        }
        if (placable != null)
        {
            CurrentPlacable = PlacementManager.Instance.PlaceAt(placable, transform.localPosition);
            CurrentPlacable.gameObject.AddComponent<RelayClickable>().ParentClickable = this;
        }
    }

    public override void Click()
    {
        //UIManager.Instance.GetBuildUI(buildUI).Show(this);
    }
}