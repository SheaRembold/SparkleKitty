using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookUI : MonoBehaviour
{
    [System.Serializable]
    public class TabData
    {
        public Toggle Tab;
        public ListPageUI ListPage;
        public ItemPageUI ItemPage;
        public PlacableDataType PlacableDataType;
    }
    
    public UIManager uiManager;
    [SerializeField]
    TabData[] tabs;
    [SerializeField]
    Button backButton;
    [SerializeField]
    Button debugButton;

    int current;

    GameObject buttonFlash;

    List<GameObject> requs = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].ItemPage.Book = this;
            tabs[i].ListPage.Book = this;
            tabs[i].Tab.onValueChanged.AddListener(OnToggleTab);
        }
        OnToggleTab(true);

        HelpManager.Instance.onCompleteTutorialStep += OnCompleteTutorialStep;
    }

    void OnCompleteTutorialStep(TutorialStep currentStep)
    {
        if (HelpManager.Instance.CurrentStep == TutorialStep.PlaceTreat)
        {
        }
    }

    public void OnToggleTab(bool value)
    {
        if (value)
        {
            for (int i = 0; i < tabs.Length; i++)
            {
                if (tabs[i].Tab.isOn)
                {
                    uiManager.ShowUI(tabs[i].ListPage.gameObject);
                    break;
                }
            }
        }
    }

    public void ShowItem(PlacableData item)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i].PlacableDataType == item.DataType)
            {
                tabs[i].Tab.isOn = true;
                tabs[i].ItemPage.SetItem(item);
                uiManager.ShowUI(tabs[i].ItemPage.gameObject);
                break;
            }
        }
    }

    public void SetInteractable(bool value)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].Tab.interactable = value;
        }
        backButton.interactable = value;
        debugButton.interactable = value;
        if (buttonFlash != null)
            Destroy(buttonFlash);
    }

    public void FlashTab(PlacableDataType tabType)
    {
        if (buttonFlash != null)
            Destroy(buttonFlash);
        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i].PlacableDataType == tabType)
            {
                tabs[i].Tab.interactable = true;
                if (buttonFlash == null)
                    buttonFlash = Instantiate(uiManager.flashPrefab);
                buttonFlash.GetComponent<FlashUI>().SetTarget(tabs[i].Tab.targetGraphic as Image);
            }
            else
            {
                tabs[i].Tab.interactable = false;
            }
        }
        backButton.interactable = false;
        debugButton.interactable = false;
    }
}