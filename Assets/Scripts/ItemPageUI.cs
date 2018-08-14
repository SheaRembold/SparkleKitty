using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPageUI : MonoBehaviour
{
    [SerializeField]
    protected Text NameLabel;
    [SerializeField]
    Text CountLabel;
    [SerializeField]
    GameObject RequPrefab;
    [SerializeField]
    Text UnknownLabel;
    [SerializeField]
    Transform Content;
    [SerializeField]
    protected Text PageLabel;
    [SerializeField]
    Button UseButton;
    [SerializeField]
    Text UseButtonText;
    [SerializeField]
    Button CraftButton;
    [SerializeField]
    Text CraftButtonText;

    [SerializeField]
    PlacableDataType currentType = PlacableDataType.Tower;
    protected int current;

    List<GameObject> requs = new List<GameObject>();
    
    public void SetItem(PlacableData item)
    {
        current = System.Array.IndexOf(DataManager.Instance.GetAllData(currentType), item);
    }

    private void Awake()
    {
        PlacementManager.Instance.onFinishedPlacing += OnFinishedPlacing;
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
    
    protected virtual void UpdateRecipe()
    {
        if (currentType == PlacableDataType.Treat)
            UpdateRecipe(DataManager.Instance.Treats);
        else if (currentType == PlacableDataType.Toy)
            UpdateRecipe(DataManager.Instance.Toys);
        else if (currentType == PlacableDataType.Tower)
            UpdateUpgrade(DataManager.Instance.Towers);
        //PlacementManager.Instance.GetPlayArea().GetInArea(PlacableDataType.Tower)[0].Data as UpgradableData
    }

    void UpdateRecipe(BuildableData[] buildables)
    {
        if (current < 0)
            current = buildables.Length - 1;
        else if (current >= buildables.Length)
            current = 0;

        NameLabel.text = buildables[current].Name;
        PageLabel.text = (current + 1) + " / " + buildables.Length;
        int count = PlayerManager.Instance.GetInventoryCount(buildables[current]);
        CountLabel.text = count.ToString();
        if (buildables[current].Attached)
        {
            if (PlacementManager.Instance.CurrentAttached != null && PlacementManager.Instance.CurrentAttached.Data == buildables[current])
            {
                UseButtonText.text = "Return";
                UseButton.interactable = true;
            }
            else
            {
                UseButtonText.text = "Use";
                UseButton.interactable = count > 0;
            }
        }
        else
        {
            if (PlacementManager.Instance.GetPlayArea().GetInArea(buildables[current]).Count > 0)
            {
                UseButtonText.text = "Return";
                UseButton.interactable = true;
            }
            else
            {
                UseButtonText.text = "Place";
                UseButton.interactable = count > 0;
            }
        }

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
            bool requMissing = false;
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
                    requObj.transform.SetParent(Content, false);
                    (requObj.transform as RectTransform).anchoredPosition = new Vector2(125, -210 - 200 * requIndex);
                    requs.Add(requObj);
                }
                int invCount = PlayerManager.Instance.GetInventoryCount(pair.Key);
                requObj.GetComponentInChildren<Image>().sprite = pair.Key.Icon;
                requObj.GetComponentInChildren<Text>().text = invCount + " / " + pair.Value;
                requIndex++;
                if (invCount < pair.Value)
                    requMissing = true;
            }

            CraftButton.gameObject.SetActive(true);
            //CraftButtonText.text = "Craft";
            CraftButton.interactable = !requMissing;
        }
        else
        {
            for (int i = 0; i < requs.Count; i++)
                requs[i].SetActive(false);
            UnknownLabel.gameObject.SetActive(true);
            UnknownLabel.text = "????";
            CraftButton.gameObject.SetActive(false);
        }
    }

    void UpdateUpgrade(UpgradableData[] upgradables)
    {
        if (current < 0)
            current = upgradables.Length - 1;
        else if (current >= upgradables.Length)
            current = 0;
        PageLabel.text = (current + 1) + " / " + upgradables.Length;

        UpgradableData upgradable = null;
        for (int i = 0; i < upgradables.Length; i++)
        {
            if (upgradables[i].Upgrade == upgradables[current])
            {
                upgradable = upgradables[i];
                break;
            }
        }

        int count = PlayerManager.Instance.GetInventoryCount(upgradables[current]);
        bool inPlayArea = PlacementManager.Instance.GetPlayArea().GetInArea(upgradables[current]).Count > 0;
        CountLabel.text = count.ToString();
        UseButtonText.text = "Place";
        UseButton.interactable = count > 0 && !inPlayArea;

        //if (upgradable.Upgrade != null)
        { 
            NameLabel.text = upgradables[current].Name;

            for (int i = 0; i < requs.Count; i++)
                requs[i].SetActive(false);

            Dictionary<PlacableData, int> requCounts = new Dictionary<PlacableData, int>();
            if (upgradable != null)
            {
                requCounts.Add(upgradable, 1);
                for (int i = 0; i < upgradable.UpgradeRequirements.Length; i++)
                {
                    if (requCounts.ContainsKey(upgradable.UpgradeRequirements[i]))
                        requCounts[upgradable.UpgradeRequirements[i]]++;
                    else
                        requCounts.Add(upgradable.UpgradeRequirements[i], 1);
                }
            }
            else
            {
                for (int i = 0; i < upgradables[current].BuildRequirements.Length; i++)
                {
                    if (requCounts.ContainsKey(upgradables[current].BuildRequirements[i]))
                        requCounts[upgradables[current].BuildRequirements[i]]++;
                    else
                        requCounts.Add(upgradables[current].BuildRequirements[i], 1);
                }
            }

            int requIndex = 0;
            GameObject requObj = null;
            /*if (requIndex < requs.Count)
            {
                requObj = requs[requIndex];
            }
            else
            {
                requObj = Instantiate(RequPrefab);
                requObj.transform.SetParent(Content, false);
                (requObj.transform as RectTransform).anchoredPosition = new Vector2(125, -210 - 200 * requIndex);
                requs.Add(requObj);
            }
            requObj.SetActive(false);
            requIndex++;
            UnknownLabel.gameObject.SetActive(true);
            UnknownLabel.text = upgradable.Name;*/
            UnknownLabel.gameObject.SetActive(false);

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
                int invCount = PlayerManager.Instance.GetInventoryCount(pair.Key) + PlacementManager.Instance.GetPlayArea().GetInArea(pair.Key).Count;
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
        /*else
        {
            NameLabel.text = upgradable.Name + " not upgradable";
            for (int i = 0; i < requs.Count; i++)
                requs[i].SetActive(false);
            UnknownLabel.gameObject.SetActive(false);
            CraftButton.gameObject.SetActive(false);
        }*/
    }

    public void Craft()
    {
        if (currentType == PlacableDataType.Treat)
            Craft(DataManager.Instance.Treats);
        else if (currentType == PlacableDataType.Toy)
            Craft(DataManager.Instance.Toys);
        else if (currentType == PlacableDataType.Tower)
            Upgrade(DataManager.Instance.Towers);
    }

    void Craft(BuildableData[] buildables)
    {
        for (int i = 0; i < buildables[current].BuildRequirements.Length; i++)
        {
            PlayerManager.Instance.RemoveInventory(buildables[current].BuildRequirements[i]);
        }
        PlayerManager.Instance.AddInventory(buildables[current]);

        UpdateRecipe(buildables);
    }

    void Upgrade(UpgradableData[] upgradables)
    {
        UpgradableData upgradable = null;
        for (int i = 0; i < upgradables.Length; i++)
        {
            if (upgradables[i].Upgrade == upgradables[current])
            {
                upgradable = upgradables[i];
                break;
            }
        }

        if (upgradable != null)
        {
            for (int i = 0; i < upgradable.UpgradeRequirements.Length; i++)
            {
                PlayerManager.Instance.RemoveInventory(upgradable.UpgradeRequirements[i]);
            }
            if (PlacementManager.Instance.GetPlayArea().GetPlacementLocation().CurrentPlacable.Data == upgradable)
            {
                PlacementManager.Instance.GetPlayArea().GetPlacementLocation().SetPlacable(upgradables[current]);
            }
            else
            {
                PlayerManager.Instance.RemoveInventory(upgradable);
                PlayerManager.Instance.AddInventory(upgradables[current]);
            }
        }
        else
        {
            for (int i = 0; i < upgradables[current].BuildRequirements.Length; i++)
            {
                PlayerManager.Instance.RemoveInventory(upgradables[current].BuildRequirements[i]);
            }
            PlayerManager.Instance.AddInventory(upgradables[current]);
        }

        UpdateUpgrade(upgradables);
    }

    public void UseItem()
    {
        if (currentType == PlacableDataType.Treat)
        {
            UseItem(DataManager.Instance.Treats);
        }
        else if (currentType == PlacableDataType.Toy)
        {
            UseItem(DataManager.Instance.Toys);
        }
        else if (currentType == PlacableDataType.Tower)
        {
            PlaceItem(DataManager.Instance.Towers);
        }
    }

    void UseItem(BuildableData[] buildables)
    {
        if (buildables[current].Attached)
        {
            if (PlacementManager.Instance.CurrentAttached != null && PlacementManager.Instance.CurrentAttached.Data == buildables[current])
            {
                PlacementManager.Instance.RemoveAttached();
                UseButtonText.text = "Use";
            }
            else
            {
                PlacementManager.Instance.SetAttached(buildables[current]);
                UseButtonText.text = "Return";
            }
        }
        else
        {
            List<Placable> inArea = PlacementManager.Instance.GetPlayArea().GetInArea(buildables[current]);
            if (inArea.Count > 0)
            {
                for (int i = 0; i < inArea.Count; i++)
                {
                    PlayerManager.Instance.AddInventory(inArea[i].Data);
                    PlacementManager.Instance.Remove(inArea[i]);
                }
                int count = PlayerManager.Instance.GetInventoryCount(buildables[current]);
                CountLabel.text = count.ToString();
                UseButtonText.text = "Place";
            }
            else
            {
                PlacementManager.Instance.StartPlacing(buildables[current]);
            }
        }
    }

    void PlaceItem(UpgradableData[] upgradables)
    {
        PlayerManager.Instance.AddInventory(PlacementManager.Instance.GetPlayArea().GetPlacementLocation().CurrentPlacable.Data);
        PlacementManager.Instance.GetPlayArea().GetPlacementLocation().SetPlacable(upgradables[current]);
        PlayerManager.Instance.RemoveInventory(upgradables[current]);
        int count = PlayerManager.Instance.GetInventoryCount(upgradables[current]);
        CountLabel.text = count.ToString();
        UseButton.interactable = false;
    }
    
    void OnFinishedPlacing(Placable placable)
    {
        UpdateRecipe();
    }
}