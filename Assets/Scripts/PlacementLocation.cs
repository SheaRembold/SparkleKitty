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
    [System.NonSerialized]
    public PlacementArea Owner;
    [SerializeField]
    ParticleSystem placeEfect;

    public void SetPlacable(PlacableData placable, bool showEffect = false)
    {
        if (CurrentPlacable != null)
        {
            PlacementManager.Instance.Remove(Owner, CurrentPlacable);
            CurrentPlacable = null;
        }
        if (placable != null)
        {
            CurrentPlacable = PlacementManager.Instance.PlaceAt(Owner, placable, transform.localPosition);
            CurrentPlacable.gameObject.AddComponent<RelayClickable>().ParentClickable = this;
            if (showEffect && placeEfect != null)
                placeEfect.Play();
                
        }
    }

    public override void Click(RaycastHit hit)
    {
        //UIManager.Instance.GetBuildUI(buildUI).Show(this);
    }
}