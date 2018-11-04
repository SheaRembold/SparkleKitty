using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ListPageUI : MonoBehaviour
{
    [SerializeField]
    protected GameObject ButtonPrefab;
    [SerializeField]
    protected Transform Content;
    [SerializeField]
    protected ItemPageUI ItemPage;
    [SerializeField]
    protected PlacableDataType PlacableDataType;
    
    [SerializeField]
    UIManager uiManager;

    [System.NonSerialized]
    public BookUI Book;

    GameObject buttonFlash;
    List<Button> buttons = new List<Button>();

    protected virtual void OnEnable()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }

        PlacableData[] allData = DataManager.Instance.GetAllData(PlacableDataType);
        Button button;
        for (int i = 0; i < allData.Length; i++)
        {
            PlacableData itemData = allData[i];
            if (i < buttons.Count)
            {
                button = buttons[i];
                button.gameObject.SetActive(true);
            }
            else
            {
                GameObject obj = Instantiate(ButtonPrefab);
                obj.transform.SetParent(Content, false);
                (obj.transform as RectTransform).anchoredPosition = new Vector2(175 + (i % 3) * 250, -125 - (i / 3) * 250);
                button = obj.GetComponent<Button>();
                buttons.Add(button);
            }
            PlacableButton image = button.GetComponent<PlacableButton>();
            image.SetPlacable(itemData);
            button.onClick.AddListener(new UnityEngine.Events.UnityAction(() => { ShowItem(itemData); }));
        }

        if ((HelpManager.Instance.CurrentStep == TutorialStep.CraftToy || HelpManager.Instance.CurrentStep == TutorialStep.PlaceToy) && PlacableDataType != PlacableDataType.Toy)
        {
            if (buttonFlash == null)
                Destroy(buttonFlash);
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].interactable = false;
            Book.FlashTab(PlacableDataType.Toy);
        }
        else if (HelpManager.Instance.CurrentStep == TutorialStep.PlaceTreat || HelpManager.Instance.CurrentStep == TutorialStep.CraftToy || HelpManager.Instance.CurrentStep == TutorialStep.PlaceToy)
        {
            if (buttonFlash == null)
                buttonFlash = Instantiate(uiManager.flashPrefab);
            buttonFlash.GetComponent<FlashUI>().SetTarget(buttons[0].targetGraphic as Image);
            buttons[0].interactable = true;
            for (int i = 1; i < buttons.Count; i++)
                buttons[i].interactable = false;
            Book.SetInteractable(false);
        }
        else
        {
            if (buttonFlash != null)
                Destroy(buttonFlash);
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].interactable = true;
            Book.SetInteractable(true);
        }
    }

    protected void ShowItem(PlacableData item)
    {
        ItemPage.SetItem(item);
        uiManager.ShowUI(ItemPage.gameObject);
    }

    public void PrevPage()
    {

    }

    public void NextPage()
    {

    }
}