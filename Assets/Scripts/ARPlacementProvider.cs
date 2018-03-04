using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityARInterface;

public class ARPlacementProvider : PlacementProvider
{
    public ARPlacementProvider()
    {
        sceneObj = GameObject.Instantiate(Resources.Load<GameObject>("ARScene"));
    }

    public override bool GetPlane(out BoundedPlane plane)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("ARGameObject")))
        {
            Transform parent = hit.transform.parent;
            plane = new BoundedPlane() { center = parent.position, rotation = parent.rotation, extents = new Vector2(parent.localScale.x * 0.5f, parent.localScale.z * 0.5f) };
            return true;
        }

        plane = new BoundedPlane();
        return false;
    }
}