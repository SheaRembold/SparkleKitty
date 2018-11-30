using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChasable
{
    Vector3 ChasePosition { get; }
    ToyController Controller { get; }
    float Attraction(CatController cat);
}
