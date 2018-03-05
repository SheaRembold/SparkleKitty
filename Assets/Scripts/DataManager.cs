using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    
    public CatData[] Cats;
    public PlacableData[] Towers;
    public BuildableData[] Toys;
    public BuildableData[] Treats;
    public PlacableData[] Components;
    
    private void Awake()
    {
        Instance = this;
    }
}