using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public virtual bool PlaceInScene(GameObject obj)
    {
        return false;
    }

    public virtual bool GetPlane(out UnityARInterface.BoundedPlane plane)
    {
        plane = new UnityARInterface.BoundedPlane();
        return false;
    }
}