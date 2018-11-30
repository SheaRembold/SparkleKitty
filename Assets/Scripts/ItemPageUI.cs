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
    protected Button prevButton;
    [SerializeField]
    protected Button nextButton;

    [SerializeField]
    protected List<GameObject> requs = new List<GameObject>();
    
    [System.NonSerialized]
    public BookUI Book;

    protected GameObject buttonFlash;

    public virtual void SetItem(PlacableData item)
    {
        current = System.Array.IndexOf(DataManager.Instance.GetAllData(currentType), item);
    }
    
    private void OnEnable()
    {
        PlayerManager.Instance.onInventoryChange += UpdateRecipe;
        UpdateRecipe();

        Book.ShowPage(currentType);
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

        bool hasRecipe = PlayerManager.Instance.HasRecipe(buildables[current]);

        NameLabel.text = buildables[current].Name;
        ItemImage.sprite = buildables[current].Icon;
        ItemImage.color = (hasRecipe || buildables[current].Unlimited) ? Color.white : Color.black;
        PageLabel.text = (current + 1) + " / " + buildables.Length;
        int count = PlayerManager.Instance.GetInventoryCount(buildables[current]);
        int areaCount = 0;
        CountLabel.text = buildables[current].Unlimited ? "\u221E" : count.ToString();
        if (buildables[current].Attached)
        {
            if (PlacementManager.Instance.CurrentAttached != null && PlacementManager.Instance.CurrentAttached.Data == buildables[current])
            {
                SetUseButton("Return");
                UseButton.interactable = true;
                areaCount = 1;
            }
            else
            {
                SetUseButton("Use");
                UseButton.interactable = count > 0 || buildables[current].Unlimited;
            }
        }
        else
        {
            if (PlacementManager.Instance.GetPlayArea().GetInArea(buildables[current]).Count > 0)
            {
                SetUseButton("Return");
                UseButton.interactable = true;
                areaCount = 1;
            }
            else
            {
                SetUseButton("Place");
                UseButton.interactable = count > 0 || buildables[current].Unlimited;
            }
        }

        if (buildables[current].Unlimited)
            HealthImage.fillAmount = 0f;
        else
            HealthImage.fillAmount = 1f - (PlayerManager.Instance.GetItemHealth(buildables[current]) - (count + areaCount - 1));

        if (buildables[current].Unlimited)
        {
            for (int i = 0; i < requs.Count; i++)
            {
                requs[i].SetActive(false);
            }
            CraftButton.gameObject.SetActive(false);
        }
        else
        {
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
                CraftButton.gameObject.SetActive(false);
                CraftButton.interactable = false;
            }
        }

        if ((HelpManager.Instance.CurrentStep == TutorialStep.CraftToy || HelpManager.Instance.CurrentStep == TutorialStep.PlaceToy) && buildables[current].name != "FeltMouse")
        {
            if (buttonFlash == null)
                Destroy(buttonFlash);
            UseButton.interactable = false;
            CraftButton.interactable = false;
            prevButton.interactable = false;
            nextButton.interactable = false;
            Book.FlashTab(PlacableDataType.Toy);
        }
        else if (HelpManager.Instance.CurrentStep == TutorialStep.PlaceTreat || HelpManager.Instance.CurrentStep == TutorialStep.PlaceToy)
        {
            if (buttonFlash == null)
                buttonFlash = Instantiate(Book.uiManager.flashPrefab);
            buttonFlash.GetComponent<FlashUI>().SetTarget(UseButton.targetGraphic as Image);
            UseButton.interactable = true;
            CraftButton.interactable = false;
            prevButton.interactable = false;
            nextButton.interactable = false;
            Book.SetInteractable(false);
        }
        else if (HelpManager.Instance.CurrentStep == TutorialStep.CraftToy)
        {
            if (buttonFlash == null)
                buttonFlash = Instantiate(Book.uiManager.flashPrefab);
            buttonFlash.GetComponent<FlashUI>().SetTarget(CraftButton.targetGraphic as Image);
            UseButton.interactable = false;
            CraftButton.interactable = true;
            prevButton.interactable = false;
            nextButton.interactable = false;
            Book.SetInteractable(false);
        }
        else
        {
            if (buttonFlash != null)
                Destroy(buttonFlash);
            prevButton.interactable = true;
            nextButton.interactable = true;
            Book.SetInteractable(true);
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
        PlayerManager.Instance.AddItemHealth(buildables[current], 1f);
        PlayerManager.Instance.AddInventory(buildables[current]);

        if (HelpManager.Instance.CurrentStep == TutorialStep.CraftToy)
        {
            HelpManager.Instance.CompleteTutorialStep(TutorialStep.CraftToy);
            UpdateRecipe(buildables);
        }
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
                CountLabel.text = buildables[current].Unlimited ? "\u221E" : count.ToString();
                SetUseButton("Place");
            }
            else
            {
                PlacementManager.Instance.StartPlacing(buildables[current]);
                if (buttonFlash != null)
                    Destroy(buttonFlash);
            }
        }
    }
}