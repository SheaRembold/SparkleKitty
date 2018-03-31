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
    
    List<PlacableData> inventory = new List<PlacableData>();

    public int InventoryCount { get { return inventory.Count; } }
    public PlacableData GetInventory(int index) { return inventory[index]; }

    public delegate void OnInventoryChange();
    public event OnInventoryChange onInventoryChange;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlacementManager.Instance.onAreaSet += OnAreaSet;

        inventory.AddRange(startingInventory);
    }

    void OnAreaSet()
    {
        for (int i = 0; i < startingInArea.Length; i++)
        {
            PlacementManager.Instance.PlaceAt(startingInArea[i].Placable, startingInArea[i].Position);
        }
        //UIManager.Instance.ShowSpeechUI(GetInArea("SparkleKitty").transform);
    }

    public void AddInventory(PlacableData item)
    {
        inventory.Add(item);
        if (onInventoryChange != null)
            onInventoryChange();
    }

    public void RemoveInventory(PlacableData item)
    {
        inventory.Remove(item);
        if (onInventoryChange != null)
            onInventoryChange();
    }
    
    List<PlacableData> tempPlacableDatas = new List<PlacableData>();
    public List<PlacableData> GetInventoryItems(PlacableDataType dataType)
    {
        tempPlacableDatas.Clear();
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].DataType == dataType)
                tempPlacableDatas.Add(inventory[i]);
        }
        return tempPlacableDatas;
    }

    public int GetInventoryCount(PlacableData data)
    {
        int count = 0;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == data)
                count++;
        }
        return count;
    }
}