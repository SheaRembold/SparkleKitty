using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPageUI : MonoBehaviour
{
    [SerializeField]
    protected Text NameLabel;
    [SerializeField]
    protected Image ItemImage;
    [SerializeField]
    protected Image HealthImage;
    [SerializeField]
    protected Text CountLabel;
    [SerializeField]
    protected GameObject RequPrefab;
    [SerializeField]
    protected Text UnknownLabel;
    [SerializeField]
    protected Transform Content;
    [SerializeField]
    protected Text PageLabel;
    [SerializeField]
    protected Button UseButton;
    [SerializeField]
    protected Text UseButtonText;
    [SerializeField]
    protected Button CraftButton;
    [SerializeField]
    protected Text CraftButtonText;
    [SerializeField]
    protected Sprite PlaceIcon;
    [SerializeField]
    protected Sprite PlaceIconPressed;
    [SerializeField]
    protected Sprite PlaceIconDisabled;
    [SerializeField]
    protected Sprite ReturnIcon;
    [SerializeField]
    protected Sprite ReturnIconPressed;
    [SerializeField]
    protected Sprite ReturnIconDisabled;

    [SerializeField]
    protected PlacableDataType currentType = PlacableDataType.Toy;
    protected int current;

    [SerializeField]
    protected List<GameObject> requs = new List<GameObject>();

    [System.NonSerialized]
    public BookUI Book;
    
    public virtual void SetItem(PlacableData item)
    {
        current = System.Array.IndexOf(DataManager.Instance.GetAllData(currentType), item);
    }
    
    private void OnEnable()
    {
        PlayerManager.Instance.onInventoryChange += UpdateRecipe;
        UpdateRecipe();
    }

    private void OnDisable()
    {
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.onInventoryChange -= UpdateRecipe;
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
    
    void SetUseButton(string mode)
    {
        UseButtonText.text = mode;
        if (mode == "Place" || mode == "Use")
        {
            UseButton.spriteState = new SpriteState() { pressedSprite = PlaceIconPressed, disabledSprite = PlaceIconDisabled };
            (UseButton.targetGraphic as Image).sprite = PlaceIcon;
        }
        else if (mode == "Return")
        {
            UseButton.spriteState = new SpriteState() { pressedSprite = ReturnIconPressed, disabledSprite = ReturnIconDisabled };
            (UseButton.targetGraphic as Image).sprite = ReturnIcon;
        }
    }

    protected virtual void UpdateRecipe()
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
        ItemImage.sprite = buildables[current].Icon;
        PageLabel.text = (current + 1) + " / " + buildables.Length;
        int count = PlayerManager.Instance.GetInventoryCount(buildables[current]);
        CountLabel.text = count.ToString();
        if (count == 0)
            HealthImage.fillAmount = 1f;
        else
            HealthImage.fillAmount = 1f - PlayerManager.Instance.GetItemHealth(buildables[current]);
        if (buildables[current].Attached)
        {
            if (PlacementManager.Instance.CurrentAttached != null && PlacementManager.Instance.CurrentAttached.Data == buildables[current])
            {
                SetUseButton("Return");
                UseButton.interactable = true;
            }
            else
            {
                SetUseButton("Use");
                UseButton.interactable = count > 0;
            }
        }
        else
        {
            if (PlacementManager.Instance.GetPlayArea().GetInArea(buildables[current]).Count > 0)
            {
                SetUseButton("Return");
                UseButton.interactable = true;
            }
            else
            {
                SetUseButton("Place");
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
                requObj.GetComponentInChildren<Image>().color = Color.white;
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
            {
                requs[i].SetActive(true);
                requs[i].GetComponentInChildren<Image>().sprite = null;
                requs[i].GetComponentInChildren<Image>().color = Color.gray;
                requs[i].GetComponentInChildren<Text>().text = "? / ?";
            }
            UnknownLabel.gameObject.SetActive(false);
            //UnknownLabel.text = "????";
            //CraftButton.gameObject.SetActive(false);
            CraftButton.interactable = false;
        }
    }
    
    public virtual void Craft()
    {
        if (currentType == PlacableDataType.Treat)
            Craft(DataManager.Instance.Treats);
        else if (currentType == PlacableDataType.Toy)
            Craft(DataManager.Instance.Toys);
    }

    void Craft(BuildableData[] buildables)
    {
        for (int i = 0; i < buildables[current].BuildRequirements.Length; i++)
        {
            PlayerManager.Instance.RemoveInventory(buildables[current].BuildRequirements[i]);
        }
        PlayerManager.Instance.AddInventory(buildables[current]);

        //UpdateRecipe(buildables);
    }
    
    public virtual void UseItem()
    {
        if (currentType == PlacableDataType.Treat)
        {
            UseItem(DataManager.Instance.Treats);
        }
        else if (currentType == PlacableDataType.Toy)
        {
            UseItem(DataManager.Instance.Toys);
        }
    }

    void UseItem(BuildableData[] buildables)
    {
        if (buildables[current].Attached)
        {
            if (PlacementManager.Instance.CurrentAttached != null && PlacementManager.Instance.CurrentAttached.Data == buildables[current])
            {
                PlacementManager.Instance.RemoveAttached();
                SetUseButton("Use");
            }
            else
            {
                PlacementManager.Instance.SetAttached(buildables[current]);
                SetUseButton("Return");
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
                SetUseButton("Place");
            }
            else
            {
                PlacementManager.Instance.StartPlacing(buildables[current]);
            }
        }
    }
}