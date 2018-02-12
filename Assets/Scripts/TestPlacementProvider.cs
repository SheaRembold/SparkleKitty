using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityARInterface;

public class TestPlacementProvider : PlacementProvider
{
    public TestPlacementProvider()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("TestScene"));
    }
    
    public override bool GetPlane(out BoundedPlane plane)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("ARGameObject")))
        {
            plane = new BoundedPlane() { center = hit.point, rotation = hit.transform.rotation, extents = new Vector2(10, 10) };
            return true;
        }

        plane = new BoundedPlane();
        return false;
    }
}