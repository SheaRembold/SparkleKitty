using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlacementProvider
{
    protected GameObject sceneObj;

    public virtual void TurnOff()
    {
        sceneObj.SetActive(false);
    }

    public virtual void TurnOn()
    {
        sceneObj.SetActive(true);
    }

    public virtual void FinishInit()
    {

    }

    public virtual bool IsReady()
    {
        return true;
    }

    public abstract bool GetPlane(out UnityARInterface.BoundedPlane plane);
}