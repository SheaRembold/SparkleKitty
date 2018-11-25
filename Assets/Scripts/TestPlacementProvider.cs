using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;

public class TestPlacementProvider : PlacementProvider
{
    public TestPlacementProvider()
    {
        sceneObj = GameObject.Instantiate(Resources.Load<GameObject>("TestScene"));

        holdAttachPoint = sceneObj.GetComponentInChildren<Camera>().transform.Find("AttachPoint");
        viewAttachPoint = holdAttachPoint;
    }
    
    public override bool GetPlane(out BoundedPlane plane)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("ARGameObject")))
        {
            plane = new BoundedPlane() { Center = hit.point, Pose = new Pose(hit.point, hit.transform.rotation), Size = new Vector2(10, 10) };
            return true;
        }

        plane = new BoundedPlane();
        return false;
    }
}