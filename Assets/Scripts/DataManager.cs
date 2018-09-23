using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlacableDataType { Cat, Tower, Toy, Treat, TowerComponent, ToyComponent, TreatComponent, TowerRecipe, ToyRecipe, TreatRecipe }

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public const int saveVersion = 3;

    public CatData[] Cats;
    public UpgradableData[] Towers;
    public BuildableData[] Toys;
    public BuildableData[] Treats;
    public PlacableData[] TowerComponents;
    public PlacableData[] ToyComponents;
    public PlacableData[] TreatComponents;
    public RecipeData[] TowerRecipes;
    public RecipeData[] ToyRecipes;
    public RecipeData[] TreatRecipes;

    Dictionary<string, PlacableData> allData = new Dictionary<string, PlacableData>();
    Dictionary<MaterialType, int> towerLevels = new Dictionary<MaterialType, int>();

    private void Awake()
    {
        Instance = this;

        AddData(Cats);
        AddData(Towers);
        AddData(Toys);
        AddData(Treats);
        AddData(TowerComponents);
        AddData(ToyComponents);
        AddData(TreatComponents);
        AddData(TowerRecipes);
        AddData(ToyRecipes);
        AddData(TreatRecipes);

        MaterialType[] materialTypes = System.Enum.GetValues(typeof(MaterialType)) as MaterialType[];
        for (int i = 0; i < materialTypes.Length; i++)
            towerLevels.Add(materialTypes[i], 0);
    }

    void AddData(PlacableData[] dataArray)
    {
        for (int i = 0; i < dataArray.Length; i++)
        {
            allData.Add(dataArray[i].name, dataArray[i]);
        }
    }

    public PlacableData GetData(string name)
    {
        if (allData.ContainsKey(name))
            return allData[name];

        return null;
    }

    public PlacableData[] GetAllData(PlacableDataType dataType)
    {
        if (dataType == PlacableDataType.Cat)
            return Cats;
        else if (dataType == PlacableDataType.Tower)
            return Towers;
        else if (dataType == PlacableDataType.Toy)
            return Toys;
        else if (dataType == PlacableDataType.Treat)
            return Treats;
        else if (dataType == PlacableDataType.TowerComponent)
            return TowerComponents;
        else if (dataType == PlacableDataType.ToyComponent)
            return ToyComponents;
        else if (dataType == PlacableDataType.TreatComponent)
            return TreatComponents;
        else if (dataType == PlacableDataType.TowerRecipe)
            return TowerRecipes;
        else if (dataType == PlacableDataType.ToyRecipe)
            return ToyRecipes;
        else if (dataType == PlacableDataType.TreatRecipe)
            return TreatRecipes;
        else
            return null;
    }

    public UpgradableData GetTower(MaterialType type, int level)
    {
        for (int i = 0; i < Towers.Length; i++)
        {
            if (Towers[i].MaterialType == type && Towers[i].Level == level)
                return Towers[i];
        }
        return null;
    }

    public void SetTowerLevel(MaterialType type, int level)
    {
        towerLevels[type] = level;
    }

    public int GetTowerLevel(MaterialType type)
    {
        return towerLevels[type];
    }
}