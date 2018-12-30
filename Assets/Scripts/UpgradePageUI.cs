using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePageUI : ItemPageUI
{
    [SerializeField]
    protected List<GameObject> levels = new List<GameObject>();

    MaterialType currentMat;

    string[] levelNames = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };
    
    public override void SetItem(PlacableData item)
    {
        currentMat = (item as UpgradableData).MaterialType;
        current = (int)currentMat;
    }
    
    protected override void UpdateRecipe()
    {
        if (currentType == PlacableDataType.Tower)
            UpdateUpgrade();//DataManager.Instance.Towers);
    }
    
    void UpdateUpgrade()//UpgradableData[] upgradables)
    {
        int matCount = System.Enum.GetNames(typeof(MaterialType)).Length;
        if (current < 0)
            current = matCount - 1;
        else if (current >= matCount)
            current = 0;
        currentMat = (MaterialType)current;
        PageLabel.text = (current + 1) + " / " + matCount;

        int currentLevel = DataManager.Instance.GetTowerLevel(currentMat);
        UpgradableData upgradable = DataManager.Instance.GetTower(currentMat, currentLevel == 0 ? 1 : currentLevel);
        
        bool inPlayArea = PlacementManager.Instance.GetPlayArea().GetInArea(upgradable).Count > 0;
        CountLabel.text = levelNames[currentLevel];
        //SetUseButton("Place");
        UseButton.interactable = currentLevel > 0 && !inPlayArea;
        
        NameLabel.text = upgradable.Name;
        ItemImage.sprite = upgradable.Icon;
        if (currentLevel == 0)
            ItemImage.color = Color.black;
        else
            ItemImage.color = Color.white;

        for (int i = 0; i < currentLevel; i++)
            levels[i].GetComponentInChildren<Image>().color = Color.white;
        for (int i = currentLevel; i < levels.Count; i++)
            levels[i].GetComponentInChildren<Image>().color = Color.gray;

        for (int i = 0; i < requs.Count; i++)
            requs[i].SetActive(false);

        Dictionary<PlacableData, int> requCounts = new Dictionary<PlacableData, int>();
        if (currentLevel == 0)
        {
            for (int i = 0; i < upgradable.BuildRequirements.Length; i++)
            {
                if (requCounts.ContainsKey(upgradable.BuildRequirements[i]))
                    requCounts[upgradable.BuildRequirements[i]]++;
                else
                    requCounts.Add(upgradable.BuildRequirements[i], 1);
            }
        }
        else
        {
            for (int i = 0; i < upgradable.UpgradeRequirements.Length; i++)
            {
                if (requCounts.ContainsKey(upgradable.UpgradeRequirements[i]))
                    requCounts[upgradable.UpgradeRequirements[i]]++;
                else
                    requCounts.Add(upgradable.UpgradeRequirements[i], 1);
            }
        }

        UnknownLabel.gameObject.SetActive(false);

        if (requCounts.Count == 0)
        {
            CraftButton.gameObject.SetActive(false);
        }
        else
        {
            int requIndex = 0;
            GameObject requObj = null;
            bool requMissing = false;
            foreach (KeyValuePair<PlacableData, int> pair in requCounts)
            {
                requObj = null;
                if (requIndex < requs.Count)
                {
                    requObj = requs[requIndex];
                    requObj.SetActive(true);
                }
                else
                {
                    requObj = Instantiate(RequPrefab);
                    requObj.transform.SetParent(Content, false);
                    (requObj.transform as RectTransform).anchoredPosition = new Vector2(125, -210 - 200 * requIndex);
                    requs.Add(requObj);
                }
                int invCount = PlayerManager.Instance.GetInventoryCount(pair.Key);// + PlacementManager.Instance.GetPlayArea().GetInArea(pair.Key).Count;
                requObj.GetComponentInChildren<Image>().sprite = pair.Key.Icon;
                requObj.GetComponentInChildren<Text>().text = invCount + " / " + pair.Value;
                requIndex++;
                if (invCount < pair.Value)
                    requMissing = true;
            }

            CraftButton.gameObject.SetActive(true);
            //CraftButtonText.text = "Upgrade";
            CraftButton.interactable = !requMissing;
        }
    }

    public override void Craft()
    {
        if (currentType == PlacableDataType.Tower)
            Upgrade();//DataManager.Instance.Towers);
    }
    
    void Upgrade()//UpgradableData[] upgradables)
    {
        int currentLevel = DataManager.Instance.GetTowerLevel(currentMat);
        UpgradableData upgradable = DataManager.Instance.GetTower(currentMat, currentLevel == 0 ? 1 : currentLevel);

        if (currentLevel == 0)
        {
            for (int i = 0; i < upgradable.BuildRequirements.Length; i++)
            {
                PlayerManager.Instance.RemoveInventory(upgradable.BuildRequirements[i]);
            }
            PlayerManager.Instance.AddInventory(upgradable);
            DataManager.Instance.SetTowerLevel(currentMat, upgradable.Level);
        }
        else
        {
            for (int i = 0; i < upgradable.UpgradeRequirements.Length; i++)
            {
                PlayerManager.Instance.RemoveInventory(upgradable.UpgradeRequirements[i]);
            }
            if (PlacementManager.Instance.GetPlayArea().GetPlacementLocation().CurrentPlacable.Data == upgradable)
            {
                PlacementManager.Instance.GetPlayArea().SetTower(upgradable.Upgrade, true);
            }
            else
            {
                PlayerManager.Instance.RemoveInventory(upgradable);
                PlayerManager.Instance.AddInventory(upgradable.Upgrade);
            }
            DataManager.Instance.SetTowerLevel(currentMat, upgradable.Upgrade.Level);
        }

        //UpdateUpgrade(upgradables);
    }

    public override void UseItem()
    {
        if (currentType == PlacableDataType.Tower)
        {
            PlaceItem();//DataManager.Instance.Towers);
        }
    }
    
    void PlaceItem()//UpgradableData[] upgradables)
    {
        int currentLevel = DataManager.Instance.GetTowerLevel(currentMat);
        UpgradableData upgradable = DataManager.Instance.GetTower(currentMat, currentLevel);

        PlayerManager.Instance.AddInventory(PlacementManager.Instance.GetPlayArea().GetPlacementLocation().CurrentPlacable.Data);
        PlacementManager.Instance.GetPlayArea().SetTower(upgradable);
        PlayerManager.Instance.RemoveInventory(upgradable);
        UseButton.interactable = false;
    }
}