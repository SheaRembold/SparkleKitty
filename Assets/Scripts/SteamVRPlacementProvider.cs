using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;

public class SteamVRPlacementProvider : PlacementProvider
{
    SteamVR_TrackedObject leftTrackedObj;
    SteamVR_TrackedObject rightTrackedObj;

    public SteamVRPlacementProvider()
    {
        sceneObj = GameObject.Instantiate(Resources.Load<GameObject>("SteamVRScene"));

        SteamVR_ControllerManager controllerManager = sceneObj.GetComponentInChildren<SteamVR_ControllerManager>();
        leftTrackedObj = controllerManager.left.GetComponent<SteamVR_TrackedObject>();
        rightTrackedObj = controllerManager.right.GetComponent<SteamVR_TrackedObject>();
        holdAttachPoint = rightTrackedObj.transform.Find("AttachPoint");
        viewAttachPoint = sceneObj.GetComponentInChildren<Camera>().transform.Find("AttachPoint");
    }
    
    public override bool GetPlane(out BoundedPlane plane)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("ARGameObject")))
        {
            plane = new BoundedPlane() { Center = hit.point, Pose = new Pose(hit.point, hit.transform.rotation), Size = new Vector2(10, 10) };
            return true;
        }

        plane = new BoundedPlane();
        return false;
    }

    public override bool GetClickDown()
    {
        return leftTrackedObj.gameObject.activeSelf && SteamVR_Controller.Input((int)leftTrackedObj.index).GetPressDown(SteamVR_Controller.ButtonMask.Trigger)
            || rightTrackedObj.gameObject.activeSelf && SteamVR_Controller.Input((int)rightTrackedObj.index).GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
    }

    public override bool GetClickUp()
    {
        return leftTrackedObj.gameObject.activeSelf && SteamVR_Controller.Input((int)leftTrackedObj.index).GetPressUp(SteamVR_Controller.ButtonMask.Trigger)
            || rightTrackedObj.gameObject.activeSelf && SteamVR_Controller.Input((int)rightTrackedObj.index).GetPressUp(SteamVR_Controller.ButtonMask.Trigger);
    }

    public override Ray GetClickRay()
    {
        return new Ray(rightTrackedObj.transform.position, rightTrackedObj.transform.forward);
    }

    public override Ray GetPlaceRay()
    {
        return new Ray(rightTrackedObj.transform.position, rightTrackedObj.transform.forward);
    }
}