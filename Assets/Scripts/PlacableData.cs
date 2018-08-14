using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlacableData : ScriptableObject
{
    public string Name;
    public PlacableDataType DataType = PlacableDataType.Cat;
    public Sprite Icon;
    public GameObject Prefab;
    public bool Attached;
}
