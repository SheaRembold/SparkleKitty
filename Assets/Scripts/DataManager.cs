using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    
    public PlacableData[] Cats;
    public PlacableData[] Towers;
    public PlacableData[] Toys;
    public PlacableData[] Treats;
    public PlacableData[] Components;
    
    private void Awake()
    {
        Instance = this;
    }


}