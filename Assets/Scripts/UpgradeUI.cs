using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField]
    Text RequsLabel;
    [SerializeField]
    Button UpgradeButton;

    Placable placable;
    UpgradableData upgradeData;
    bool upgradeAllowed;

    public void ShowUpgrade(UpgradeClickable upgradable)
    {
        placable = upgradable;
        upgradeData = upgradable.Data as UpgradableData;

        if (upgradeData.Upgrade != null)
        {
            Dictionary<PlacableData, int> upRequs = new Dictionary<PlacableData, int>();
            for (int i = 0; i < upgradeData.UpgradeRequirements.Length; i++)
            {
                if (upRequs.ContainsKey(upgradeData.UpgradeRequirements[i]))
                    upRequs[upgradeData.UpgradeRequirements[i]]++;
                else
                    upRequs.Add(upgradeData.UpgradeRequirements[i], 1);
            }
            upgradeAllowed = true;
            string requsText = "";
            foreach (KeyValuePair<PlacableData, int> pair in upRequs)
            {
                int invCount = PlayerManager.Instance.GetInventoryCount(pair.Key);
                requsText += invCount + " / " + pair.Value + " " + pair.Key.Name;
                if (invCount < pair.Value)
                    upgradeAllowed = false;
            }
            RequsLabel.text = requsText;
            UpgradeButton.interactable = upgradeAllowed;
        }
        else
        {
            upgradeAllowed = false;

            RequsLabel.text = "Not upgradable";
            UpgradeButton.interactable = upgradeAllowed;
        }
    }

    public void Upgrade()
    {
        if (upgradeAllowed)
        {
            for (int i = 0; i < upgradeData.UpgradeRequirements.Length; i++)
            {
                PlayerManager.Instance.RemoveInventory(upgradeData.UpgradeRequirements[i]);
            }
            PlacementManager.Instance.Replace(placable, upgradeData.Upgrade);
            UIManager.Instance.GoBack();
        }
    }
}