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

        if (HelpManager.Instance.CurrentStep == TutorialStep.PlaceTreat)
        {
            if (buttonFlash == null)
                buttonFlash = Instantiate(uiManager.flashPrefab);
            buttonFlash.GetComponent<FlashUI>().SetTarget(buttons[0].targetGraphic as Image);
            buttons[0].interactable = true;
            for (int i = 1; i < buttons.Count; i++)
                buttons[i].interactable = false;
        }
        else
        {
            if (buttonFlash != null)
                Destroy(buttonFlash);
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].interactable = true;
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