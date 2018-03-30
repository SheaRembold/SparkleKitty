using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlacableDataType { Cat, Tower, Toy, Treat, Component, ToyRecipe, TreatRecipe }

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    
    public CatData[] Cats;
    public PlacableData[] Towers;
    public BuildableData[] Toys;
    public BuildableData[] Treats;
    public PlacableData[] Components;
    public RecipeData[] ToyRecipes;
    public RecipeData[] TreatRecipes;
    
    private void Awake()
    {
        Instance = this;
    }

    public PlacableData[] GetData(PlacableDataType dataType)
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