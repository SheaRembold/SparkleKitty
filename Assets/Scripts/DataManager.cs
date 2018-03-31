using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlacableDataType { Cat, Tower, Toy, Treat, Component, ToyRecipe, TreatRecipe }

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    
    public CatData[] Cats;
    public UpgradableData[] Towers;
    public BuildableData[] Toys;
    public BuildableData[] Treats;
    public PlacableData[] Components;
    public RecipeData[] ToyRecipes;
    public RecipeData[] TreatRecipes;

    Dictionary<string, PlacableData> allData = new Dictionary<string, PlacableData>();

    private void Awake()
    {
        Instance = this;

        AddData(Cats);
        AddData(Towers);
        AddData(Toys);
        AddData(Treats);
        AddData(Components);
        AddData(ToyRecipes);
        AddData(TreatRecipes);
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
        else if (dataType == PlacableDataType.Component)
            return Components;
        else if (dataType == PlacableDataType.ToyRecipe)
            return ToyRecipes;
        else if (dataType == PlacableDataType.TreatRecipe)
            return TreatRecipes;
        else
            return null;
    }
}