using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Placable : MonoBehaviour
{
    [System.NonSerialized]
    public PlacableData Data;

    public virtual void AddedToArea()
    {
    }

    public virtual void RemovedFromArea()
    {
    }
}