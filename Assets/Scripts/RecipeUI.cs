using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
    [SerializeField]
    Text NameLabel;
    [SerializeField]
    Text RequsLabel;
    [SerializeField]
    Text PageLabel;
    [SerializeField]
    Toggle TreatTab;
    [SerializeField]
    Toggle ToyTab;

    PlacableDataType currentType = PlacableDataType.Treat;
    int current;

    private void Awake()
    {
        TreatTab.onValueChanged.AddListener(OnToggleTreat);
        ToyTab.onValueChanged.AddListener(OnToggleToy);
    }

    private void OnEnable()
    {
        UpdateRecipe();
    }

    public void PrevRecipe()
    {
        current--;
        UpdateRecipe();
    }

    public void NextRecipe()
    {
        current++;
        UpdateRecipe();
    }

    void UpdateRecipe()
    {
        if (currentType == PlacableDataType.Treat)
            UpdateRecipe(DataManager.Instance.Treats);
        else if (currentType == PlacableDataType.Toy)
            UpdateRecipe(DataManager.Instance.Toys);
    }

    void UpdateRecipe(BuildableData[] buildables)
    {
        if (current < 0)
            current = buildables.Length - 1;
        else if (current >= buildables.Length)
            current = 0;

        NameLabel.text = buildables[current].Name;
        PageLabel.text = (current + 1) + " / " + buildables.Length; 
        string requs = "";
        bool hasRecipe = false;
        for (int i = 0; i < PlayerManager.Instance.InventoryCount; i++)
        {
            RecipeData recipe = PlayerManager.Instance.GetInventory(i) as RecipeData;
            if (recipe != null && recipe.Product == buildables[current])
            {
                hasRecipe = true;
                break;
            }
        }
        if (hasRecipe)
        {
            for (int i = 0; i < buildables[current].BuildRequirements.Length; i++)
            {
                requs += buildables[current].BuildRequirements[i].Name + "\n";
            }
        }
        else
        {
            requs = "????";
        }
        RequsLabel.text = requs;
    }

    public void OnToggleTreat(bool value)
    {
        currentType = PlacableDataType.Treat;
        UpdateRecipe(DataManager.Instance.Treats);
    }

    public void OnToggleToy(bool value)
    {
        currentType = PlacableDataType.Toy;
        UpdateRecipe(DataManager.Instance.Toys);
    }
}