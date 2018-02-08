using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityARInterface;

public class TestSceneManager : SceneController
{
    public override bool PlaceInScene(GameObject obj)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        {
            obj.transform.position = hit.point;
            obj.transform.rotation = hit.transform.rotation;
            obj.transform.localScale = hit.transform.parent.localScale;
            return true;
        }

        return false;
    }

    public override bool GetPlane(out BoundedPlane plane)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("ARGameObject")))
        {
            plane = new BoundedPlane() { center = hit.point, rotation = hit.transform.rotation, extents = new Vector2(10, 10) };
            return true;
        }
        
        return base.GetPlane(out plane);
    }
}