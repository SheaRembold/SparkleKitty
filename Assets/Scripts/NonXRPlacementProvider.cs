using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;

public class NonXRPlacementProvider : PlacementProvider
{
    public NonXRPlacementProvider()
    {
        sceneObj = GameObject.Instantiate(Resources.Load<GameObject>("NonXRScene"));

        holdAttachPoint = sceneObj.GetComponentInChildren<Camera>().transform.Find("AttachPoint");
        viewAttachPoint = holdAttachPoint;
    }
    
    public override bool GetPlane(out BoundedPlane plane)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("ARGameObject")))
        {
            plane = new BoundedPlane() { Center = hit.point, Pose = new Pose(hit.point, hit.transform.rotation), Size = new Vector2(10f, 10f) };
            return true;
        }

        plane = new BoundedPlane();
        return false;
    }

    public override Vector3 BookOffset { get { return new Vector3(0f, 0.05f, 0.1f); } }

    public override Ray GetPlaceRay()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return Camera.main.ScreenPointToRay(Input.mousePosition);
#else
        if (Input.touchCount > 0)
            return Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        return new Ray(Vector3.up, Vector3.up);
#endif
    }
}