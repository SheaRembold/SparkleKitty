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
    List<string> shownHelp = new List<string>();

    private void Awake()
    {
        Instance = this;

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
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] is UpgradableData)
            {
                UpgradableData upgradable = inventory[i] as UpgradableData;
                if (upgradable.Level > DataManager.Instance.GetTowerLevel(upgradable.MaterialType))
                    DataManager.Instance.SetTowerLevel(upgradable.MaterialType, upgradable.Level);
            }
        }

        if (File.Exists(Application.persistentDataPath + "/help.txt"))
        {
            string[] helpNames = File.ReadAllLines(Application.persistentDataPath + "/help.txt");
            shownHelp.AddRange(helpNames);
        }
    }

    public void ShowHelp(string name)
    {
        if (!shownHelp.Contains(name))
        {
            shownHelp.Add(name);

            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            for (int i = 0; i < shownHelp.Count; i++)
            {
                builder.AppendLine(shownHelp[i]);
            }
            File.WriteAllText(Application.persistentDataPath + "/help.txt", builder.ToString());
        }
    }

    public bool HasShownHelp(string name)
    {
        return shownHelp.Contains(name);
    }

    public void AddInventory(PlacableData item)
    {
        inventory.Add(item);
        invDirty = true;
    }

    public void RemoveInventory(PlacableData item)
    {
        inventory.Remove(item);
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

    public float GetItemHealth(PlacableData data)
    {
        return 1f;
    }

    private void LateUpdate()
    {
        if (invDirty)
        {
            if (onInventoryChange != null)
                onInventoryChange();
            Save();
        }
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