using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementArea : MonoBehaviour
{
    protected List<Placable> placedInArea = new List<Placable>();

    public virtual void AddToArea(Placable placable)
    {
        if (!placedInArea.Contains(placable))
            placedInArea.Add(placable);
    }

    public virtual void RemoveFromArea(Placable placable)
    {
        placedInArea.Remove(placable);
    }

    public virtual void MoveInArea(Placable placable)
    {
    }
}