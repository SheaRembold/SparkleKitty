using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneManager : SceneManager
{
    public override bool PlaceInScene(GameObject obj)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        {
            obj.transform.position = hit.point;
            return true;
        }

        return false;
    }
}