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

    int current;

    private void OnEnable()
    {
        UpdateRecipe();
    }

    public void PrevRecipe()
    {
        current--;
        if (current < 0)
            current = DataManager.Instance.Toys.Length - 1;
        UpdateRecipe();
    }

    public void NextRecipe()
    {
        current++;
        if (current >= DataManager.Instance.Toys.Length)
            current = 0;
        UpdateRecipe();
    }

    void UpdateRecipe()
    {
        NameLabel.text = DataManager.Instance.Toys[current].Name;
        string requs = "";
        for (int i = 0; i < DataManager.Instance.Toys[current].BuildRequirements.Length; i++)
        {
            requs += DataManager.Instance.Toys[current].BuildRequirements[i].Name + "\n";
        }
        RequsLabel.text = requs;
    }
}