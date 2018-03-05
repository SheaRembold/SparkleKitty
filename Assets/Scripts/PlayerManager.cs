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
        PlacementManager.Instance.onAreaSet += OnAreaSet;
        PlacementManager.Instance.onPlaced += OnPlacedPlacable;
        PlacementManager.Instance.onMoved += OnMovedPlacable;
        PlacementManager.Instance.onRemoved += OnRemovedPlacable;

        inventory.AddRange(startingInventory);
    }

    void OnAreaSet()
    {
        for (int i = 0; i < startingInArea.Length; i++)
        {
            PlacementManager.Instance.PlacingAt(startingInArea[i].Placable, startingInArea[i].Position);
        }
        //UIManager.Instance.ShowSpeechUI(GetInArea("SparkleKitty").transform);
    }

    Placable GetInArea(string name)
    {
        for (int i=0;i< placedInArea.Count;i++)
        {
            if (placedInArea[i].Data.name == name)
                return placedInArea[i];
        }
        return null;
    }

    void OnPlacedPlacable(Placable placable)
    {
        if (!placedInArea.Contains(placable))
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

    List<Placable> validPlacables = new List<Placable>();
    public Placable GetRandomInArea<T>() where T : PlacableData
    {
        validPlacables.Clear();
        for (int i = 0; i < placedInArea.Count; i++)
        {
            if (placedInArea[i].Data is T)
                validPlacables.Add(placedInArea[i]);
        }
        if (validPlacables.Count > 0)
            return validPlacables[Random.Range(0, validPlacables.Count)];

        return null;
    }

	public void CheckCats(){

	}
}