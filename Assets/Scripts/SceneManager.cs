using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public virtual bool PlaceInScene(GameObject obj)
    {
        return false;
    }
}