using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlacementProvider
{
    public Transform holdAttachPoint;
    public Transform viewAttachPoint;

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

    public virtual bool GetClickDown()
    {
        return Input.GetMouseButtonDown(0);
    }

    public virtual bool GetClickUp()
    {
        return Input.GetMouseButtonUp(0);
    }

    public virtual Ray GetClickRay()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    public virtual Ray GetPlaceRay()
    {
        return new Ray(Camera.main.transform.position, Camera.main.transform.forward);
    }
}