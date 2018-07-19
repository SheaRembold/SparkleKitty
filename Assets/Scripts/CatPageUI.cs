using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatPageUI : ItemPageUI
{
    protected override void UpdateRecipe()
    {
        if (current < 0)
            current = DataManager.Instance.Cats.Length - 1;
        else if (current >= DataManager.Instance.Cats.Length)
            current = 0;

        NameLabel.text = DataManager.Instance.Cats[current].Name;
        PageLabel.text = (current + 1) + " / " + DataManager.Instance.Cats.Length;
    }
}