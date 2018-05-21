using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
    [SerializeField]
    GameObject ButtonPrefab;
    [SerializeField]
    Transform Content;
    int buttonCount;


    private void Start()
    {
        for (int i = 0; i < DataManager.Instance.TowerComponents.Length; i++)
            AddButton(DataManager.Instance.TowerComponents[i]);
        for (int i = 0; i < DataManager.Instance.ToyComponents.Length; i++)
            AddButton(DataManager.Instance.ToyComponents[i]);
        for (int i = 0; i < DataManager.Instance.TreatComponents.Length; i++)
            AddButton(DataManager.Instance.TreatComponents[i]);
        for (int i = 0; i < DataManager.Instance.TowerRecipes.Length; i++)
            AddButton(DataManager.Instance.TowerRecipes[i]);
        for (int i = 0; i < DataManager.Instance.ToyRecipes.Length; i++)
            AddButton(DataManager.Instance.ToyRecipes[i]);
        for (int i = 0; i < DataManager.Instance.TreatRecipes.Length; i++)
            AddButton(DataManager.Instance.TreatRecipes[i]);
    }

    void AddButton(PlacableData componentData)
    {
        GameObject obj = Instantiate(ButtonPrefab);
        obj.transform.SetParent(Content, false);
        (obj.transform as RectTransform).anchoredPosition = new Vector2(240 + (buttonCount % 4) * 200, 640 - (buttonCount / 4) * 200);
        obj.GetComponent<InventoryDebugButton>().SetPlacable(componentData);
        buttonCount++;
    }
}