using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    
    [SerializeField]
    PlacableData[] startingInventory;
    
    List<PlacableData> inventory = new List<PlacableData>();

    public int InventoryCount { get { return inventory.Count; } }
    public PlacableData GetInventory(int index) { return inventory[index]; }

    public delegate void OnInventoryChange();
    public event OnInventoryChange onInventoryChange;
    bool invDirty;

    Dictionary<PlacableData, Texture2D> catPhotos = new Dictionary<PlacableData, Texture2D>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/inventory.txt"))
        {
            string[] invNames = File.ReadAllLines(Application.persistentDataPath + "/inventory.txt");
            for (int i = 0; i < invNames.Length; i++)
            {
                PlacableData item = DataManager.Instance.GetData(invNames[i]);
                if (item != null)
                    inventory.Add(item);
            }
        }
        else
        {
            inventory.AddRange(startingInventory);
        }
    }

    public void AddInventory(PlacableData item)
    {
        inventory.Add(item);
        if (onInventoryChange != null)
            onInventoryChange();
        invDirty = true;
    }

    public void RemoveInventory(PlacableData item)
    {
        inventory.Remove(item);
        if (onInventoryChange != null)
            onInventoryChange();
        invDirty = true;
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

    private void LateUpdate()
    {
        if (invDirty)
            Save();
    }

    void Save()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for (int i = 0; i < inventory.Count; i++)
        {
            builder.AppendLine(inventory[i].name);
        }
        File.WriteAllText(Application.persistentDataPath + "/inventory.txt", builder.ToString());

        invDirty = false;
    }

    public void AddCatPhoto(PlacableData data, Texture2D photo)
    {
        if (catPhotos.ContainsKey(data))
            catPhotos[data] = photo;
        else
            catPhotos.Add(data, photo);
    }
    
    public Texture2D GetCatPhoto(PlacableData data)
    {
        if (catPhotos.ContainsKey(data))
            return catPhotos[data];

        if (File.Exists(Application.persistentDataPath + "/" + data.name + ".png"))
        {
            Texture2D photo = new Texture2D(2, 2, TextureFormat.RGB24, false);
            photo.LoadImage(File.ReadAllBytes(Application.persistentDataPath + "/" + data.name + ".png"));
            catPhotos.Add(data, photo);
            return photo;
        }

        return null;
    }
}