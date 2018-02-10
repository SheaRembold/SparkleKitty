using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlacementProvider
{
    public abstract bool GetPlane(out UnityARInterface.BoundedPlane plane);
}