using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;

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

    public virtual Transform GetRoot()
    {
        return null;
    }

    public abstract bool GetPlane(out BoundedPlane plane);

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

    public virtual Vector3 BookOffset { get { return new Vector3(0f, 0f, 0.3f); } }
}