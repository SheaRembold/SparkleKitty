using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlacedInst
{
    public PlacableData Placable;
    public Vector3 Position;
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField]
    PlacedInst[] startingInArea;
    [SerializeField]
    PlacableData[] startingInventory;

    List<Placable> placedInArea = new List<Placable>();
    List<PlacableData> inventory = new List<PlacableData>();
    
    public int InventoryCount { get { return inventory.Count; } }
    public PlacableData GetInventory(int index) { return inventory[index]; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlacementManager.Instance.onPlaced += OnPlacedPlacable;
        PlacementManager.Instance.onMoved += OnMovedPlacable;
        PlacementManager.Instance.onRemoved += OnRemovedPlacable;
        
        for (int i = 0; i < startingInArea.Length; i++)
        {
            PlacementManager.Instance.PlacingAt(startingInArea[i].Placable, startingInArea[i].Position);
        }
        inventory.AddRange(startingInventory);
    }

    void OnPlacedPlacable(Placable placable)
    {
        placedInArea.Add(placable);
    }

    void OnMovedPlacable(Placable placable)
    {
    }

    void OnRemovedPlacable(Placable placable)
    {
        placedInArea.Remove(placable);
    }

    public void AddInventory(PlacableData item)
    {
        inventory.Add(item);
    }

    public void RemoveInventory(PlacableData item)
    {
        inventory.Remove(item);
    }
}