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
    Dictionary<PlacableData, float> itemHealth = new Dictionary<PlacableData, float>();

    private void Awake()
    {
        Instance = this;

        if (File.Exists(Application.persistentDataPath + "/inventory_" + DataManager.saveVersion + ".txt"))
        {
            string[] invNames = File.ReadAllLines(Application.persistentDataPath + "/inventory_" + DataManager.saveVersion + ".txt");
            int i = 0;
            while (i < invNames.Length && invNames[i] != "----")
            {
                PlacableData item = DataManager.Instance.GetData(invNames[i]);
                if (item != null)
                    inventory.Add(item);
                i++;
            }
            i++;
            while (i < invNames.Length)
            {
                PlacableData item = DataManager.Instance.GetData(invNames[i]);
                if (item != null)
                    itemHealth.Add(item, float.Parse(invNames[i + 1]));
                i += 2;
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

    public bool HasRecipe(PlacableData data)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            RecipeData recipe = inventory[i] as RecipeData;
            if (recipe != null && recipe.Product == data)
            {
                return true;
            }
        }
        return false;
    }

    public void AddItemHealth(PlacableData data, float health)
    {
        if (itemHealth.ContainsKey(data))
            itemHealth[data] += health;
        else
            itemHealth[data] = health;

        if (itemHealth[data] < 0f)
            itemHealth[data] = 0f;
    }
    
    public float GetItemHealth(PlacableData data)
    {
        if (itemHealth.ContainsKey(data))
            return itemHealth[data];

        return 0f;
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

    public void MarkAsDirty()
    {
        invDirty = true;
    }

    void Save()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for (int i = 0; i < inventory.Count; i++)
        {
            builder.AppendLine(inventory[i].name);
        }
        builder.AppendLine("----");
        foreach (KeyValuePair<PlacableData, float> pair in itemHealth)
        {
            builder.AppendLine(pair.Key.name);
            builder.AppendLine(pair.Value.ToString());
        }
        File.WriteAllText(Application.persistentDataPath + "/inventory_" + DataManager.saveVersion + ".txt", builder.ToString());

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