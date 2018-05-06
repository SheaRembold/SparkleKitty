using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
    [SerializeField]
    Text NameLabel;
    [SerializeField]
    GameObject RequPrefab;
    [SerializeField]
    Text UnknownLabel;
    [SerializeField]
    Text PageLabel;
    [SerializeField]
    Toggle TreatTab;
    [SerializeField]
    Toggle ToyTab;
    [SerializeField]
    Toggle TowerTab;

    PlacableDataType currentType = PlacableDataType.Treat;
    int current;

    List<GameObject> requs = new List<GameObject>();

    private void Awake()
    {
        TreatTab.onValueChanged.AddListener(OnToggleTreat);
        ToyTab.onValueChanged.AddListener(OnToggleToy);
        TowerTab.onValueChanged.AddListener(OnToggleTower);
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
        else if (currentType == PlacableDataType.Tower)
            UpdateUpgrade(PlacementManager.Instance.GetPlayArea().GetInArea(PlacableDataType.Tower)[0].Data as UpgradableData);
    }

    void UpdateRecipe(BuildableData[] buildables)
    {
        if (current < 0)
            current = buildables.Length - 1;
        else if (current >= buildables.Length)
            current = 0;

        NameLabel.text = buildables[current].Name;
        PageLabel.text = (current + 1) + " / " + buildables.Length; 

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
            UnknownLabel.gameObject.SetActive(false);
            for (int i = 0; i < requs.Count; i++)
                requs[i].SetActive(false);
            Dictionary<PlacableData, int> requCounts = new Dictionary<PlacableData, int>();
            for (int i = 0; i < buildables[current].BuildRequirements.Length; i++)
            {
                if (requCounts.ContainsKey(buildables[current].BuildRequirements[i]))
                    requCounts[buildables[current].BuildRequirements[i]]++;
                else
                    requCounts.Add(buildables[current].BuildRequirements[i], 1);
            }
            int requIndex = 0;
            foreach (KeyValuePair<PlacableData, int> pair in requCounts)
            {
                GameObject requObj = null;
                if (requIndex < requs.Count)
                {
                    requObj = requs[requIndex];
                    requObj.SetActive(true);
                }
                else
                {
                    requObj = Instantiate(RequPrefab);
                    requObj.transform.SetParent(transform);
                    (requObj.transform as RectTransform).anchoredPosition = new Vector2(-300, 500 - 200 * requIndex);
                    requs.Add(requObj);
                }
                int invCount = PlayerManager.Instance.GetInventoryCount(pair.Key);
                requObj.GetComponentInChildren<Image>().sprite = pair.Key.Icon;
                requObj.GetComponentInChildren<Text>().text = invCount + " / " + pair.Value;
                requIndex++;
            }
        }
        else
        {
            for (int i = 0; i < requs.Count; i++)
                requs[i].SetActive(false);
            UnknownLabel.gameObject.SetActive(true);
            UnknownLabel.text = "????";
        }
    }

    void UpdateUpgrade(UpgradableData upgradable)
    {
        current = 0;
        PageLabel.text = (current + 1) + " / " + 1;
        if (upgradable.Upgrade != null)
        { 
            NameLabel.text = upgradable.Upgrade.Name;

            for (int i = 0; i < requs.Count; i++)
                requs[i].SetActive(false);

            Dictionary<PlacableData, int> requCounts = new Dictionary<PlacableData, int>();
            for (int i = 0; i < upgradable.UpgradeRequirements.Length; i++)
            {
                if (requCounts.ContainsKey(upgradable.UpgradeRequirements[i]))
                    requCounts[upgradable.UpgradeRequirements[i]]++;
                else
                    requCounts.Add(upgradable.UpgradeRequirements[i], 1);
            }

            int requIndex = 0;
            GameObject requObj = null;
            if (requIndex < requs.Count)
            {
                requObj = requs[requIndex];
            }
            else
            {
                requObj = Instantiate(RequPrefab);
                requObj.transform.SetParent(transform);
                (requObj.transform as RectTransform).anchoredPosition = new Vector2(-300, 500 - 200 * requIndex);
                requs.Add(requObj);
            }
            requObj.SetActive(false);
            requIndex++;
            UnknownLabel.gameObject.SetActive(true);
            UnknownLabel.text = upgradable.Name;

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
                    requObj.transform.SetParent(transform);
                    (requObj.transform as RectTransform).anchoredPosition = new Vector2(-300, 500 - 200 * requIndex);
                }
                int invCount = PlayerManager.Instance.GetInventoryCount(pair.Key);
                requObj.GetComponentInChildren<Image>().sprite = pair.Key.Icon;
                requObj.GetComponentInChildren<Text>().text = invCount + " / " + pair.Value;
                requIndex++;
            }
        }
        else
        {
            NameLabel.text = upgradable.Name + " not upgradable";
            for (int i = 0; i < requs.Count; i++)
                requs[i].SetActive(false);
            UnknownLabel.gameObject.SetActive(false);
        }
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

    public void OnToggleTower(bool value)
    {
        currentType = PlacableDataType.Tower;
        UpdateUpgrade(PlacementManager.Instance.GetPlayArea().GetInArea(PlacableDataType.Tower)[0].Data as UpgradableData);
    }
}