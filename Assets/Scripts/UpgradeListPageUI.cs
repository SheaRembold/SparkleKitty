using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeListPageUI : ListPageUI
{
    protected override void OnEnable()
    {
        for (int i = 0; i < Content.childCount; i++)
        {
            Destroy(Content.GetChild(i).gameObject);
        }

        MaterialType[] materialTypes = System.Enum.GetValues(typeof(MaterialType)) as MaterialType[];
        for (int i = 0; i < materialTypes.Length; i++)
        {
            int level = DataManager.Instance.GetTowerLevel(materialTypes[i]);
            PlacableData itemData = DataManager.Instance.GetTower(materialTypes[i], level == 0 ? 1 : level);
            GameObject obj = Instantiate(ButtonPrefab);
            obj.transform.SetParent(Content, false);
            (obj.transform as RectTransform).anchoredPosition = new Vector2(175 + (i % 3) * 250, -125 - (i / 3) * 250);
            PlacableButton image = obj.GetComponent<PlacableButton>();
            image.SetPlacable(itemData);
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(new UnityEngine.Events.UnityAction(() => { ShowItem(itemData); }));
        }
    }
}