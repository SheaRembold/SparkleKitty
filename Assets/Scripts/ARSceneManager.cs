using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityARInterface;

public class ARSceneManager : SceneController
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
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("ARGameObject")))
        {
            Transform parent = hit.transform.parent;
            plane = new BoundedPlane() { center = parent.position, rotation = parent.rotation, extents = new Vector2(parent.localScale.x * 0.5f, parent.localScale.z * 0.5f) };
            return true;
        }

        return base.GetPlane(out plane);
    }
}